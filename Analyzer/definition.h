//
//  defenitions.h
//  task3
//
//  Created by Дмитрий Кузнецов on 19.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#ifndef task3_definitions_h
#define task3_definitions_h

#include <iostream>

enum enumTokenType{
    token_error = 0,
    token_command = 1,
    token_operand = 2,
    token_declarator = 3,
    token_number = 4,
    token_string = 5,
    token_identifier = 6,
    token_comment = 7,
    token_eol = 8,
    token_eof = 9,
};

enum enumToken{
    token_unknown = 0,
    token_if = 100,           //"IF"
    token_then = 101,         //"THEN"
    token_else = 102,         //"ELSE"
    token_write = 103,        //"WRITE"
    token_writeln = 104,      //"WRITELN"
    token_read = 105,         //"READ"
    token_readln = 106,      //"READLN"
    token_for = 107,         //"FOR"
    token_to = 108,          //"TO"
    token_until = 109,       //"UNTIL"
    token_while = 110,       //"WHILE"
  //token_as = 111,          //"AS"
    token_and = 112,         //"AND"
    token_or = 113,          //"OR"
    token_not = 114,         //"NOT"
    token_begin = 115,       //"BEGIN"
    token_end = 116,         //"END"
    token_const = 117,       //"CONST"
    token_program = 118,     //"PROGRAM"
    token_procedure = 119,   //"PROCEDURE"
    token_function = 120,    //"FUNCTION"
    token_array = 121,       //"ARRAY"
  //token_case = 122,        //"CASE"
    token_goto = 123,        //"GOTO"
    token_div = 124,         //"DIV"
  //token_label = 125,       //"LABEL"
    token_mod = 126,         //"MOD"
    token_repeat = 127,      //"REPEAT"
  //token_set = 128,         //"SET"
    token_str = 129,         //"STRING"
  //token_type = 130,        //"TYPE"
  //token_unit = 131,        //"UNIT"
    token_xor = 132,         //"XOR"
    token_with = 133,        //"WITH"
    token_nil = 134,         //"NULL"
    token_do = 135,          //"DO"
    token_var = 136,         //"VAR"
    token_integer = 137,     //"INTEGER"
    token_boolean = 138,     //"BOOLEAN"
    token_real = 139,        //"REAL"
    
    token_add = 200,         //"+"
    token_sub = 201,         //"-"
    token_mul = 202,         //"*"
    token_ddiv = 203,        //"/"
    token_equal = 204,       //"="
    token_lb = 205,          //"("
    token_rb = 206,          //")"
    token_more = 207,        //">"
    token_less = 208,        //"<"
    token_semicolon = 209,   //";"
    token_comma = 210,       //","
    token_slb = 211,         //"["
    token_srb = 212,         //"]"
    token_col = 213,         //":"
    token_assign = 214,      //":="
    
    poliz_go= 300,
    poliz_fgo= 301,
    poliz_address= 302,
    poliz_arrayindexaddress = 303,
    poliz_arrayindexvalue = 304,
    poliz_eq = 305,
    poliz_label = 306
};

struct Token{
    enumToken _eToken;      // token self
    enumTokenType _tToken;  // token type
    char _getToken[32];       // scaned token name
    char _putToken[32];       // printed token name
    
    Token(){
        _tToken=token_error;
        _eToken=token_unknown;
        strcpy(_getToken,"");
        strcpy(_putToken,"");
    }
    
    Token(enumToken _etoken, enumTokenType _ttoken){
        _tToken=_ttoken;
        _eToken=_etoken;
        strcpy(_getToken,"");
        strcpy(_putToken,"");
    }
    
    Token(enumToken _etoken, enumTokenType _ttoken, char *_gettoken, char *_puttoken){
        _tToken=_ttoken;
        _eToken=_etoken;
        strcpy(_getToken,_gettoken);
        strcpy(_putToken,_puttoken);
    }
};

struct Lexeme{
    int lexNumber;
    char *lexString;
    Token lexToken;
    
    Lexeme(){
    }
    Lexeme(Token token,char *value){
        lexToken=token;
        lexNumber=atoi(value);
        lexString=strdup(value);
    }
    Lexeme(Token token,int value=0){
        lexToken=token;
        lexNumber=value;
        lexString=NULL;
    }
    friend std::ostream& operator<<(std::ostream& stream,const Lexeme& lexeme){
        stream<<lexeme.lexToken._putToken<<" "<<lexeme.lexNumber<<std::endl;
        return stream;
    }
};

#endif
