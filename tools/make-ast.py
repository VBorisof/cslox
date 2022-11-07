## Utility for generating abstract syntax trees.

class Type:
    def __init__(self, className: str, fields: str):
        self.baseClass = "Expression"
        self.className = className
        ## Split by commas and strip of extra space
        self.fields = list(map(lambda x: x.strip(' '), fields.split(',')))

    def print(self):
        print(f"public class {self.className} : {self.baseClass}")
        print(f"{{")
        for field in self.fields:
            print (f"    public {field};")
        print(f"}}")

def print_ast(types: list[Type]):
    f = open("./codegen-disclaimer.txt", 'r')

    print(f.read())

    print()
    print("#nullable disable")
    print()
    print("namespace CsLox;")
    print()
    print(f"public abstract class Expression {{ }}")
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
