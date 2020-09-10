using System;
using System.Collections.Generic;
using System.Linq;

namespace Fluent.Expression.Evaluator
{
    public enum RelationalOpType
    {
        EQ = 0,
        NE = 1,
        LT = 2,
        GT = 3,
        LTE = 4,
        GTE = 5
    }

    public enum ArithmeticOpType
    {
        ADD = 0,
        SUB = 1,
        DIV = 2,
        MUL = 3,
        MOD = 4
    }
    public enum ExpEvalResultType
    {
        INVALID = -1,
        FALSE = 0,
        TRUE = 1
    }
    public enum LogicalOpType
    {
        AND = 0,
        OR = 1
    }

    public interface IExpEval<T>
    {
        LogicalOpType With { get; set; }
        T RVal { get; set; }
        bool Evaluate(ref object LVal);
    }
    
    /// <summary>
    /// Fluent interface extension methods for ExpressionTree<T>
    /// </summary>
    public static class ExpressionBuilder
    {
        public static ExpressionTree<T> Build<T>()
        {
            return new ExpressionTree<T>() { Nodes = new List<IExpEval<T>>() };
        }
        public static ExpressionTree<T> ADD<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND)
        {
            obj.Nodes.Add(new ArithmeticOp<T> { RVal = val, With = logicalOp, Op = ArithmeticOpType.ADD });
            return obj;
        }

        public static ExpressionTree<T> DIV<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND)
        {
            obj.Nodes.Add(new ArithmeticOp<T> { RVal = val, With = logicalOp, Op = ArithmeticOpType.DIV });
            return obj;
        }

        public static ExpressionTree<T> MUL<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND)
        {
            obj.Nodes.Add(new ArithmeticOp<T> { RVal = val, With = logicalOp, Op = ArithmeticOpType.MUL });
            return obj;
        }

        public static ExpressionTree<T> SUB<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND)
        {
            obj.Nodes.Add(new ArithmeticOp<T> { RVal = val, With = logicalOp, Op = ArithmeticOpType.SUB });
            return obj;
        }

        public static ExpressionTree<T> MOD<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND)
        {
            obj.Nodes.Add(new ArithmeticOp<T> { RVal = val, With = logicalOp, Op = ArithmeticOpType.MOD });
            return obj;
        }

        public static ExpressionTree<T> EQ<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND) where T : IComparable
        {
            obj.Nodes.Add(new RelationalOp<T> { RVal = val, With = logicalOp, Op = RelationalOpType.EQ });
            return obj;
        }

        public static ExpressionTree<T> NE<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND) where T : IComparable
        {
            obj.Nodes.Add(new RelationalOp<T> { RVal = val, With = logicalOp, Op = RelationalOpType.NE });
            return obj;
        }

        public static ExpressionTree<T> GTE<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND) where T : IComparable
        {
            obj.Nodes.Add(new RelationalOp<T> { RVal = val, With = logicalOp, Op = RelationalOpType.GTE });
            return obj;
        }

        public static ExpressionTree<T> LTE<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND) where T : IComparable
        {
            obj.Nodes.Add(new RelationalOp<T> { RVal = val, With = logicalOp, Op = RelationalOpType.LTE });
            return obj;
        }

        public static ExpressionTree<T> GE<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND) where T : IComparable
        {
            obj.Nodes.Add(new RelationalOp<T> { RVal = val, With = logicalOp, Op = RelationalOpType.GT });
            return obj;
        }

        public static ExpressionTree<T> LT<T>(this ExpressionTree<T> obj, T val, LogicalOpType logicalOp = LogicalOpType.AND) where T : IComparable
        {
            obj.Nodes.Add(new RelationalOp<T> { RVal = val, With = logicalOp, Op = RelationalOpType.LT });
            return obj;
        }

        public static ExpEval<T> AddInnerExpression<T>(this ExpEval<T> obj, ExpEval<T> innerExpression, LogicalOpType logicalOp = LogicalOpType.AND) where T : struct
        {
            obj.InnerExpression = new InnerExpEval<T> { With = logicalOp, Expression = innerExpression };
            return obj;
        }
    }

    public class ExpressionTree<T>
    {
        public List<IExpEval<T>> Nodes { get; set; }
        public void Clear() => Nodes.Clear();

    }

    public class ArithmeticOp<T> : IExpEval<T>
    {
        public LogicalOpType With { get; set; } = LogicalOpType.AND;
        public T RVal { get; set; }
        public ArithmeticOpType Op { get; set; } = ArithmeticOpType.ADD;

        public bool Evaluate(ref object LVal)
        {
            bool res = true;
            try
            {
                switch (Op)
                {
                    case ArithmeticOpType.ADD:
                        LVal = (T)((dynamic)LVal + (dynamic)RVal);
                        break;
                    case ArithmeticOpType.SUB:
                        LVal = (T)((dynamic)LVal - (dynamic)RVal);
                        break;
                    case ArithmeticOpType.DIV:
                        LVal = (T)((dynamic)LVal / (dynamic)RVal);
                        break;
                    case ArithmeticOpType.MUL:
                        LVal = (T)((dynamic)LVal * (dynamic)RVal);
                        break;
                    case ArithmeticOpType.MOD:
                        LVal = (T)((dynamic)LVal % (dynamic)RVal);
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                res = false; ;
            }
            return res;
        }
    }

    public class RelationalOp<T> : IExpEval<T> where T : IComparable
    {
        public LogicalOpType With { get; set; } = LogicalOpType.AND;
        public T RVal { get; set; }
        public RelationalOpType Op { get; set; } = RelationalOpType.EQ;
        public bool Evaluate(ref object LVal)
        {
            switch (Op)
            {
                case RelationalOpType.EQ:
                    return RVal.CompareTo(LVal) == 0;
                case RelationalOpType.NE:
                    return RVal.CompareTo(LVal) != 0;
                case RelationalOpType.LT:
                    return RVal.CompareTo(LVal) > 0;
                case RelationalOpType.GT:
                    return RVal.CompareTo(LVal) < 0;
                case RelationalOpType.LTE:
                    return RVal.CompareTo(LVal) >= 0;
                case RelationalOpType.GTE:
                    return RVal.CompareTo(LVal) <= 0;
                default:
                    return false;
            }
        }

    }

    public class InnerExpEval<T> where T : struct
    {
        public LogicalOpType With { get; set; }
        public ExpEval<T> Expression { get; set; }
    }

    public class ExpEval<T> where T : struct
    {
        private static readonly List<Type> SupportedTypes = new List<Type>
        {
            typeof(float),
            typeof(bool),
            typeof(int),
            typeof(double),
            typeof(DateTime),
            typeof(long),
        };

        public Type GetDataType() => typeof(T);
        public ExpressionTree<T> ExpressionTree { get; set; }
        public InnerExpEval<T> InnerExpression { get; set; }
        public ExpEval()
        {
            try
            {
                SupportedTypes.First(item => item == typeof(T));//Throws exception for non supported types
            }
            catch
            {
                throw new InvalidOperationException($"Unsupported type : {nameof(T)}");
            }
        }

        public ExpressionTree<T> Build()
        {
            ExpressionTree = ExpressionBuilder.Build<T>();
            return ExpressionTree;
        }
        public ExpEvalResultType Evaluate(T val) => _Evaluate(val);
        public ExpEvalResultType Evaluate(string stringVal) => _Evaluate(Convert(stringVal));
        public object Convert(string stringVal)
        {
            object boxedObject = null;
            var type = typeof(T);
            if (type == typeof(bool))
            {
                bool val;
                if (bool.TryParse(stringVal, out val))
                {
                    boxedObject = val;
                }
            }
            else if (type == typeof(int))
            {
                int val;
                if (int.TryParse(stringVal, out val))
                {
                    boxedObject = val;
                }
            }
            else if (type == typeof(float))

            {
                float val;
                if (float.TryParse(stringVal, out val))
                {
                    boxedObject = val;
                }
            }
            else if (type == typeof(long))
            {
                long val;
                if (long.TryParse(stringVal, out val))
                {
                    boxedObject = val;
                }
            }
            else if (type == typeof(double))
            {
                double val;
                if (double.TryParse(stringVal, out val))
                {
                    boxedObject = val;
                }
            }
            else if (type == typeof(DateTime))
            {
                DateTime val;
                if (DateTime.TryParse(stringVal, out val))
                {
                    boxedObject = val;
                }
            }
            return boxedObject;
        }

        private ExpEvalResultType _Evaluate(object rValObject)
        {
            if (ExpressionTree == null || ExpressionTree.Nodes == null || ExpressionTree.Nodes.Count == 0)
                throw new ArgumentNullException(nameof(ExpressionTree));

            ExpEvalResultType res = ExpEvalResultType.INVALID;

            if (rValObject != null)
            {
                foreach (var op in ExpressionTree.Nodes)
                {
                    res = op.Evaluate(ref rValObject) ? ExpEvalResultType.TRUE : ExpEvalResultType.FALSE;
                    bool toBreak = false;
                    switch (res)
                    {
                        case ExpEvalResultType.FALSE:
                            {
                                if (op.With == LogicalOpType.AND)
                                    toBreak = true;
                            }
                            break;
                        case ExpEvalResultType.INVALID:
                            toBreak = true;
                            break;
                        case ExpEvalResultType.TRUE:
                            if (op.With == LogicalOpType.OR)
                                toBreak = true;
                            break;
                        default:
                            break;
                    }
                    if (toBreak)
                        break;
                }
                if (InnerExpression != null && res != ExpEvalResultType.INVALID)
                {
                    switch (InnerExpression.With)
                    {
                        case LogicalOpType.AND:
                            if (res == ExpEvalResultType.TRUE)
                                res = InnerExpression.Expression._Evaluate(rValObject);
                            break;
                        case LogicalOpType.OR:
                            if (res == ExpEvalResultType.FALSE)
                                res = InnerExpression.Expression._Evaluate(rValObject);
                            break;
                    }
                }
            }

            return res;
        }

    }
}
