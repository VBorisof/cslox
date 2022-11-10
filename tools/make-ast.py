## Utility for generating abstract syntax trees.

class Type:
    def __init__(self, className: str, fields: str):
        self.baseClass = "Expression"
        self.className = className
        ## Split by commas and strip of extra space
        self.fields = list(map(lambda x: x.strip(' '), fields.split(',')))

    def print(self):
        # Class declaration
        print(f"public class {self.className}{self.baseClass} : {self.baseClass}")
        print(f"{{")

        # Constructor
        print(f"    public {self.className}{self.baseClass}({', '.join(self.fields)})")
        print("    {")
        for field in self.fields:
            field_name = field.split(' ')[1]
            print(f"        this.{field_name} = {field_name};")

        print("    }")
        print()

        # Fields
        for field in self.fields:
            print (f"    public {field};")

        print()

        # Visitor
        print("    public override T Accept<T>(Visitor<T> visitor)")
        print("    {")
        print(f"        return visitor.Visit{self.className}Expression(this);")
        print("    }")
        print(f"}}")

def print_visitor(types: list[Type]):
    print("public interface Visitor<T>")
    print("{")
    for type in types:
        print(f"    T Visit{type.className}Expression({type.className}Expression {type.className.lower()});")
    print("}")

def print_ast(types: list[Type]):
    f = open("./codegen-disclaimer.txt", 'r')

    print(f.read())

    print()
    print("#nullable disable")
    print()
    print("namespace CsLox;")
    print()
    print_visitor(types)
    print()
    print(f"public abstract class Expression")
    print("{")
    print("    public abstract T Accept<T>(Visitor<T> visitor);")
    print("}")
    for type in types:
        print()
        type.print()
    print()

def main():
    types = [
        Type("Binary", "Expression left, LoxToken op, Expression right"),
        Type("Grouping", "Expression expr"),
        Type("Literal", "object val"),
        Type("Unary", "LoxToken op, Expression expr")
    ]
    print_ast(types)

if __name__ == '__main__':
    main()
