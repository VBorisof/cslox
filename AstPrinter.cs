using System.Text;

namespace CsLox
{
    public class AstPrinter : Visitor<string>
    {
        public string Print(Expression e)
        {
            return e.Accept(this);
        }

        public string VisitBinaryExpression(BinaryExpression binary)
        {
            return Parenthesize(binary.op.Lexeme, binary.left, binary.right);
        }

        public string VisitGroupingExpression(GroupingExpression grouping)
        {
            return Parenthesize("group", grouping.expr);
        }

        public string VisitLiteralExpression(LiteralExpression literal)
        {
            if (literal.val == null)
            {
                return "nil";
            }
            // Possible null reference? Really?
            #pragma warning disable CS8603
            return literal.val.ToString();
            #pragma warning restore CS8603
        }

        public string VisitUnaryExpression(UnaryExpression unary)
        {
            return Parenthesize(unary.op.Lexeme, unary.expr);
        }

        private string Parenthesize(string name, params Expression[] expressions)
        {
            var sb = new StringBuilder();

            sb.Append($"({name}");
            foreach (var e in expressions)
            {
                sb.Append($" {Print(e)}");
            }
            sb.Append(")");

            return sb.ToString();
        }
    }
}

