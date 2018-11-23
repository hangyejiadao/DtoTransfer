using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DtoTransfer
{
    public static class CloneHelper<TIn, TOut>
    {
        #region 基于表达式树  高性能


        //Todo 转换

        private static readonly Func<TIn, TOut> cache = GetFunc();
        private static Func<TIn, TOut> GetFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn), "p");
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            foreach (var item in typeof(TOut).GetProperties())
            {
                if (!item.CanWrite || typeof(TIn).GetProperty(item.Name) == null)
                    continue;

                MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                MemberBinding memberBinding = Expression.Bind(item, property);
                memberBindingList.Add(memberBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression });

            return lambda.Compile();
        }

        public static TOut Trans(TIn tIn)
        {
            return cache(tIn);
        }
        //Todo 更新
        public static TOut Update(TIn tin, TOut tout)
        {
            return updatecache(tin, tout);
        }
        private static readonly Func<TIn, TOut, TOut> updatecache = UpdateFunc();

        private static Func<TIn, TOut, TOut> UpdateFunc()
        {
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TIn));
            ParameterExpression outerExpression = Expression.Parameter(typeof(TOut));
            List<MemberBinding> memberBindingList = new List<MemberBinding>();

            Parallel.ForEach(typeof(TOut).GetProperties(), item =>
            {
                if (item.CanWrite)
                {
                    if (typeof(TIn).GetProperty(item.Name) == null)
                    {
                        MemberExpression property = Expression.Property(outerExpression, typeof(TOut).GetProperty(item.Name));
                        MemberBinding memberBinding = Expression.Bind(item, property);
                        memberBindingList.Add(memberBinding);
                    }
                    else
                    {
                        MemberExpression property = Expression.Property(parameterExpression, typeof(TIn).GetProperty(item.Name));
                        MemberBinding memberBinding = Expression.Bind(item, property);
                        memberBindingList.Add(memberBinding);
                    }
                }


            });


            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), memberBindingList.ToArray());
            Expression<Func<TIn, TOut, TOut>> lambda = Expression.Lambda<Func<TIn, TOut, TOut>>(memberInitExpression, new ParameterExpression[] { parameterExpression, outerExpression });

            return lambda.Compile();
        }
        #endregion


        #region  基于反射 性能低

        public static TOut TransDto(TIn tin)
        {

            var tout = Activator.CreateInstance<TOut>();
            var Pros = typeof(TIn).GetProperties().Join(typeof(TOut).GetProperties(),
                inner =>
                    new
                    {
                        Name = inner.Name,
                        PType = inner.PropertyType
                    },
                outter => new { Name = outter.Name, PType = outter.PropertyType }
                , (inner, outter) =>
                   inner
            );
            foreach (var item in Pros)
            {
                var value = item.GetValue(tin);
                if (!item.PropertyType.IsGenericType)
                {
                    //Todo 如果当前类型不是泛型类型，则为 true；否则为 false。
                    item.SetValue(tout, value == null ? null : Convert.ChangeType(value, item.PropertyType), null);
                }
                else
                {
                    Type genericTypeDefinition = item.PropertyType.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        //如果是值类型
                        item.SetValue(tout, value == null ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(item.PropertyType)));
                    }
                    else
                    {
                        //不考虑非空泛型  比如说IList 这些
                    }
                }
            }

            return tout;

        }

        public static TOut UpdateDto(TIn tin, TOut tout)
        {
            Type typeIn = typeof(TIn);
            var Pros = typeof(TIn).GetProperties().Join(typeof(TOut).GetProperties(),
                inner =>
                    new
                    {
                        Name = inner.Name,
                        PType = inner.PropertyType
                    },
                outter => new { Name = outter.Name, PType = outter.PropertyType }

                , (inner, outter) =>

                        outter
                    );


            foreach (var item in Pros)
            {
                var value = typeIn.GetProperty(item.Name).GetValue(tin);
                if (!item.PropertyType.IsGenericType)
                {
                    //Todo 如果当前类型是泛型类型，则为 true；否则为 false。
                    item.SetValue(tout, value == null ? null : Convert.ChangeType(value, item.PropertyType), null);
                }
                else
                {
                    Type genericTypeDefinition = item.PropertyType.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        //如果是可空泛型
                        item.SetValue(tout, value == null ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(item.PropertyType)));
                    }
                    else
                    {
                        //不考虑非空泛型  比如说IList 这些
                    }
                }
            }
            return tout;
        }
        #endregion

    }


    public static class CloneHelperExtension
    {

        public static TDes ForMember<TSour, TDes, TMember>(this TDes des, Expression<Func<TDes, TMember>> deskey, TSour sour, Expression<Func<TSour, TMember>> sourcekey
         )
        {
            var me = sourcekey.Compile();
            var demo = me(sour);


         


            var p = GetPropertyInfo(deskey);
            p.SetValue(des,demo);
            //CloneHelper<TDes, TDes>.Trans(des);


            return des;
        }

        public static PropertyInfo GetPropertyInfo<T, TR>(Expression<Func<T, TR>> select)
        {
            var body = select.Body;
            if (body.NodeType == ExpressionType.Convert)
            {
                var o = (body as UnaryExpression).Operand;
                return (o as MemberExpression).Member as PropertyInfo;
            }
            else if (body.NodeType == ExpressionType.MemberAccess)
            {
                return (body as MemberExpression).Member as PropertyInfo;
            }
            return null;
        }
    }





    /// <summary>
    /// AutoMapper扩展帮助类
    /// </summary>
    public static class AutoMapperHelper
    {

        private static Dictionary<Type, Type> typesDic = new Dictionary<Type, Type>();
        /// <summary>
        ///  类型映射
        /// </summary>
        public static TDes UpdateTo<T, TDes>(T obj, TDes des)
        {
            if (obj == null) return des;
            return Mapper.Map<T, TDes>(obj, des);
        }

    }
}
