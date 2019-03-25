//
//  Analizator.cpp
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#include "Analyzer.h"

const int SizeOfTokenTable=53;

int f;

Token TokenTable[SizeOfTokenTable]={
    Token ( token_if,			token_command,		"IF",		"If" ),
    Token ( token_then,			token_command,		"THEN",		"Then" ),
    Token ( token_else,			token_command,		"ELSE",		"Else" ),
    Token ( token_write,		token_command,		"WRITE",	"Write" ),
    Token ( token_writeln,		token_command,		"WRITELN",	"Writeln" ),
    Token ( token_read,			token_command,		"READ",		"Read" ),
    Token ( token_readln,		token_command,		"READLN",	"Readln" ),
    Token ( token_for,			token_command,		"FOR",		"For" ),
    Token ( token_to,			token_command,		"TO",		"To" ),
    Token ( token_until,		token_command,		"UNTIL",	"Until" ),
    Token ( token_while,		token_command,		"WHILE",	"While" ),
  //Token ( token_as,			token_command,		"AS",		"As" ),
    Token ( token_and,			token_operand,		"AND",		"Logical And" ),
    Token ( token_or,			token_operand,		"OR",		"Logical Or" ),
    Token ( token_not,			token_operand,		"NOT",		"Logical Not" ),
    Token ( token_begin,		token_command,		"BEGIN",	"Begin" ),
    Token ( token_end,			token_command,		"END",		"End" ),
    Token ( token_const,		token_declarator,	"CONST",	"Const" ),
    Token ( token_program,		token_command,		"PROGRAM",	"Program" ),
    Token ( token_procedure,	token_command,		"PROCEDURE","Procedure" ),
    Token ( token_function,		token_command,		"FUNCTION",	"Function" ),
    Token ( token_array,		token_declarator,	"ARRAY",	"Array" ),
  //Token ( token_case,			token_command,		"CASE",		"Case" ),
    Token ( token_goto,			token_command,		"GOTO",		"Go To" ),
  //Token ( token_label,		token_declarator,	"LABEL",	"Label" ),
    Token ( token_mod,			token_operand,		"MOD",		"Integer Divide" ),
    Token ( token_repeat,		token_command,		"REPEAT",	"Repeat" ),
  //Token ( token_set,			token_declarator,	"SET",		"Set" ),
    Token ( token_str,			token_declarator,	"STRING",	"String" ),
  //Token ( token_type,			token_declarator,	"TYPE",		"Type" ),
  //Token ( token_unit,			token_declarator,	"UNIT",		"Unit" ),
    Token ( token_xor,			token_operand,		"XOR",		"Logical Xor" ),
    Token ( token_with,			token_command,		"WITH",		"With" ),
    Token ( token_do,			token_command,		"DO",		"Do" ),
    Token ( token_var,			token_declarator,	"VAR",		"Variable" ),
    Token ( token_nil,			token_operand,		"NIL",		"Nil" ),
    Token ( token_integer,		token_declarator,	"INTEGER",	"Integer" ),
    Token ( token_boolean,		token_declarator,	"BOOLEAN",	"Boolean" ),
    Token ( token_real,         token_declarator,	"REAL",     "Integer" ),
    Token ( token_add,			token_operand,      "+",		"Add '+' " ),
    Token ( token_sub,			token_operand,      "-",		"Substract '-' " ),
    Token ( token_mul,			token_operand,      "*",		"Multiply '*' " ),
    Token ( token_div,			token_operand,      "/",		"Divide '/' " ),
    Token ( token_assign,		token_operand,      ":=",		"Assign ':=' " ),
    Token ( token_equal,		token_operand,      "=",		"Equal '=' " ),
    Token ( token_lb,			token_operand,      "(",		"Left Bracket '(' " ),
    Token ( token_rb,			token_operand,      ")",		"Right Bracket ')' " ),
    Token ( token_less,			token_operand,      "<",		"Less Sign '<' " ),
    Token ( token_more,			token_operand,      ">",		"More Sign '>' " ),
    Token ( token_semicolon,	token_operand,      ";",		"Semicolon ';' " ),
    Token ( token_comma,		token_operand,      ",",		"Comma ',' " ),
    Token ( token_slb,			token_operand,      "[",		"Left Square Bracket '[' " ),
    Token ( token_srb,			token_operand,      "]",		"Right Square Bracket ']' " ),
    Token ( token_col,			token_operand,      ":",		"Colon ':' " )
};

Analyzer::Analyzer(){
    SizeOfIDTable=0;
    IDTable=NULL;
}

Analyzer::~Analyzer(){
}

int Analyzer::GetSize(){
    return SizeOfIDTable;
}

int Analyzer::GetLex(char* program, long& pos){
    char Identifier[128];
    memset(Identifier, 0, 128);
    
    int i=1,j;
    bool inIDTable=false;
    
    if (program[pos] == EOF){
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_eof)));
        pos++; return 1;
    }
    
    while (isspace(program[pos])){
        pos++;
    }
    
    if (program[pos] == '\r'){
        pos++; return 0;
    }
    
    if (program[pos] == '\n'){
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_eol)));
        pos++; return 0;
    }
    
    if (program[pos]=='{'){
        j=0;
        pos++;
        do{
             if (program[pos]==EOF){
                LexemeList.AddNode(Lexeme(Token(token_unknown, token_error)));
                return 0;
             }
            Identifier[j++]=program[pos++];
        }
        while (program[pos-1]!='}');
        Identifier[j-1]='\0';
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_comment), Identifier));
        return 0;
    }
    
    if (program[pos]=='`'){
        j=0;
        pos++;
        do{
            if (program[pos]==EOF){
                LexemeList.AddNode(Lexeme(Token(token_unknown, token_error)));
                return 0;
            }
            Identifier[j++]=program[pos++];
        }
        while (program[pos-1]!='`');
        Identifier[j-1]='\0';
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_string), Identifier));
        return 0;
    }
    
    if (isalpha(program[pos])){
        *Identifier=toupper(program[pos++]);
        
        while (isalnum(program[pos])){
            *(Identifier+(i++)) = toupper(program[pos++]);
        }
        
        if (!strcmp(Identifier,"END") && program[pos]=='.'){
            LexemeList.AddNode(Lexeme(Token(token_unknown, token_eof)));
            pos++;
            return 1;
        }
        
        for(j=0;j<SizeOfTokenTable;j++){
            if (!strcmp(Identifier,TokenTable[j]._getToken)){
                LexemeList.AddNode(TokenTable[j]);
                return 0;
            }
        }
        
        for (j=0; j<SizeOfIDTable; j++){
            if (!strcmp(IDTable[j].GetName(),Identifier)){
                inIDTable=true;
            }
        }
        
        if (!inIDTable){
            AddId(Identifier);
        }
        
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_identifier),Identifier));
        
        return 0;
    }
    else if (isdigit(program[pos])){
        *Identifier=program[pos++];
        
        while (isdigit(program[pos])){
            *(Identifier+(i++)) = program[pos++];
        }
        
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_number),Identifier));
        
        return 0;
    }
    else if (strchr("[]+-*/=><():;,", program[pos])){
        *Identifier=program[pos++];
        
        while (strchr("[]+-*/=><():;,", program[pos])){
            *(Identifier+(i++)) = program[pos++];
        }
        
        int len=i;
        
        for(i=len;i>0;i--){
            for(j=0;j<SizeOfTokenTable;j++){
                if (!strcmp(Identifier,TokenTable[j]._getToken)){
                    LexemeList.AddNode(TokenTable[j]);
                    return 0;
                }
            }
            Identifier[i-1]='\0'; pos--;           
        }
        
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_error)));
    }
    else{
        LexemeList.AddNode(Lexeme(Token(token_unknown, token_error)));
    }
    
    return 0;
}

void Analyzer::GetLexemes(char* program){
    long pos=0;
    
    while (!GetLex(program,pos));
}

void Analyzer::AddId(char *name){
    for (int i=0; i<SizeOfIDTable; i++) {
        if (!strcmp(IDTable[i].GetName(),name)){
            return;
        }
    }
    
    ID *table = new ID[SizeOfIDTable+1];
    
    if (SizeOfIDTable>0)
    {
        memcpy(table,IDTable,SizeOfIDTable*sizeof(ID));
        //        delete IDTable;
    }
    
    table[SizeOfIDTable].PutName(name);
    SizeOfIDTable++;
    IDTable=table;
}

void Analyzer::ShowTable(){
    cout<<endl;
    cout<<"List of Identifiers"<<endl<<endl;
    for (int i=0; i<SizeOfIDTable; i++) {
        cout<<IDTable[i].GetName()<<endl;
    }
}

void Analyzer::ShowLexemes(){
    LexemeList.PrintList();
}

void Analyzer::CopyList(List& list){
    list=LexemeList;
}

void Analyzer::CopyTable(ID *table){
    memcpy(table,IDTable,sizeof(ID)*SizeOfIDTable);
}