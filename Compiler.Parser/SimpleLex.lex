%using Compiler.Parser;
%using QUT.Gppg;
%using System.Linq;

%namespace Compiler.Parser

Alpha 	[a-zA-Z_]
Digit   [0-9] 
AlphaDigit {Alpha}|{Digit}
INUM  {Digit}+
REALNUM {INUM}\.{INUM}
ID {Alpha}{AlphaDigit}* 


%%

{INUM} { 
  yylval.iVal = int.Parse(yytext); 
  return (int)Tokens.INUM; 
}


{ID}  { 
  int res = ScannerHelper.GetIDToken(yytext);
  if (res == (int)Tokens.ID)
    yylval.sVal = yytext;
  return res;
}

"=" { return (int)Tokens.ASSIGN; }
";" { return (int)Tokens.SEMICOLON; }
"(" { return (int)Tokens.OPENBR; }
")" { return (int)Tokens.CLOSEBR; }
"{" { return (int)Tokens.BEGIN; }
"}" { return (int)Tokens.END; }
"," { return (int)Tokens.COMMA; }
"+" { return (int)Tokens.PLUS; }
"-" { return (int)Tokens.MINUS; }
"*" { return (int)Tokens.MULT; }
"/" { return (int)Tokens.DIV; }
">" { return (int)Tokens.GT; }
"<" { return (int)Tokens.LT; }
">=" { return (int)Tokens.GE; }
"<=" { return (int)Tokens.LE; }
"==" { return (int)Tokens.EQ; }
"!=" { return (int)Tokens.NEQ; }
"!" { return (int)Tokens.NOT; }
":" { return (int)Tokens.COLON; }

[^ \t\r\n] {
	LexError();
	return (int)Tokens.EOF; // конец разбора
}

%{
  yylloc = new LexLocation(tokLin, tokCol, tokELin, tokECol); // позици€ символа (терминального или нетерминального), возвращаема€ @1 @2 и т.д.
%}

%%

public override void yyerror(string format, params object[] args) // обработка синтаксических ошибок
{
  var ww = args.Skip(1).Cast<string>().ToArray();
  string errorMsg = string.Format("({0},{1}): ¬стречено {2}, а ожидалось {3}", yyline, yycol, args[0], string.Join(" или ", ww));
  throw new SyntaxException(errorMsg);
}

public void LexError()
{
	string errorMsg = string.Format("({0},{1}): Ќеизвестный символ {2}", yyline, yycol, yytext);
    throw new LexException(errorMsg);
}

class ScannerHelper 
{
  private static Dictionary<string,int> keywords;

  static ScannerHelper() 
  {
    keywords = new Dictionary<string,int>();
    keywords.Add("while",(int)Tokens.CYCLE);
	keywords.Add("for", (int)Tokens.FOR);
	keywords.Add("print", (int)Tokens.PRINT);
	keywords.Add("if", (int)Tokens.IF);
	keywords.Add("else", (int)Tokens.ELSE);
	keywords.Add("goto", (int)Tokens.GOTO);

  }
  public static int GetIDToken(string s)
  {
    if (keywords.ContainsKey(s.ToLower())) // €зык нечувствителен к регистру
      return keywords[s];
    else
      return (int)Tokens.ID;
  }
}
