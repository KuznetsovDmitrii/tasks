//
//  Parser.cpp
//  task3
//
//  Created by Дмитрий Кузнецов on 26.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#include "Parser.h"

Parser::Parser(List& temp, ID *table, size_t size):program(1000){
    list=temp;
    tableID = new ID [128];
    err.msg = new char [128];
    memcpy(tableID, table, size);
    tableSize=size;
}

void Parser::next_lex(List& temp){
    temp.head=temp.head->next;
}

int Parser::analyze(){
    ifoperator=false;
    List temp;
    count=1;
    err.line=count;
    temp=list;
    cout<<endl;
    cout<<"Syntax Analyzer"<<endl;
    try{
        while (temp.getToken()!=token_eof) {
            sub_analyze(temp);
        }
        checkRedeclaration();
    }
    catch(const char* message){
        cout<<endl;
        cout<<"In Semantics"<<endl;
        cout<<message<<endl;
        return 1;
    }
    catch(...){
        cout<<endl;
        cout<<"Error"<<endl;
        cout<<"Line:"<<err.line<<endl;
        cout<<"In "<<err.msg<<endl;
        return 1;
    }
    cout<<endl;
    cout<<"No Errors"<<endl;
    program.print();
    return 0;
}

void Parser::sub_analyze(List& temp){
    switch(temp.getToken()){
        case token_if:
            strcpy(err.msg, "If Operator");
            ifoper(temp);
            break;
        case token_read:
            strcpy(err.msg, "Input Function");
            inputfunction(temp);
            break;
        case token_readln:
            strcpy(err.msg, "Input Function");
            inputfunction(temp);
            break;
        case token_write:
            strcpy(err.msg, "Print Function");
            outputfunction(temp);
            break;
        case token_writeln:
            strcpy(err.msg, "Print Function");
            outputfunction(temp);
            break;
        case token_array:
            strcpy(err.msg, "Array");
            arr(temp);
            break;
        case token_for:
            strcpy(err.msg, "For Cycle");
            forcycle(temp);
            break;
        case token_while:
            strcpy(err.msg, "While Cycle");
            whilecycle(temp);
            break;
        case token_identifier:
            strcpy(err.msg, "Expression");
            ident(temp);
            //expression0(temp);
            break;
        case token_eof:
            break;
        case token_eol:
            count++;
            err.line=count;
            next_lex(temp);
            break;
        default:
            strcpy(err.msg, "Expression");
            throw err;
            break;
    }
}

void Parser::checkRedeclaration(){
    int k = 0;
    int cnt = 0;
    size_t size = IDs.size();
    int* Sidarr = new int [size];
    for(int i = 0; i<size; i++){
        Sidarr[i]=IDs.top();
        IDs.pop();
    }
    for(k=0; k<size; k++){
        cnt = 0;
        for(int i = 0; i<size; i++){
            if(k==Sidarr[i]){
                cnt++;
            }
        }
        if (cnt>1) {
            throw "Redeclaration";
        }
    }
    delete [] Sidarr;
}

void Parser::checkNextBr(List & temp,Lexeme poliz){
    
    if (temp.head->lexeme.lexToken._eToken==token_slb){
        next_lex(temp);
        expression0(temp);
        tokens.pop();
        program.putLexem(poliz);
    }
    if (temp.head->lexeme.lexToken._eToken==token_srb) {
        next_lex(temp);
        return ;
    }
}

void Parser::ident(List & temp){
    tokens.push(getType(temp.head->elem));
    program.putLexem(Lexeme(poliz_address,checkTable(temp.head->elem)));
    next_lex(temp);
    checkNextBr(temp,poliz_arrayindexaddress);
    if (temp.head->lexeme.lexToken._eToken==token_assign)
    {
        tokens.push(token_assign);
        next_lex(temp);
        expression0(temp);
        program.putLexem(token_assign);
        return ;
    }
    strcpy(err.msg, "Expression");
    throw err;
}

void Parser::inputfunction(List & temp){
    next_lex(temp);
    if ((temp.head->lexeme.lexToken._eToken==token_str) && (temp.head->next->next->lexeme.lexToken._tToken==token_identifier)){
        program.putLexem(temp.head->lexeme.lexToken._eToken);
        next_lex(temp);
        next_lex(temp);
        Lexeme(poliz_address,checkTable(temp.head->elem));
        program.putLexem(token_read);
        return ;
    }
    if (temp.head->lexeme.lexToken._tToken==token_identifier){
        program.putLexem(Lexeme(poliz_address,checkTable(temp.head->elem)));
        next_lex(temp);
        program.putLexem(token_read);
        return ;
    }
    strcpy(err.msg, "Input Function");
    throw err;
}

void Parser::outputfunction(List & temp){
    do{
        next_lex(temp);
        if ((temp.head->lexeme.lexToken._eToken==token_str) || (temp.head->lexeme.lexToken._tToken==token_identifier)
            ||(temp.head->lexeme.lexToken._tToken==token_number)) {
            if (temp.head->lexeme.lexToken._tToken==token_identifier) {
                program.putLexem(Lexeme(poliz_address,checkTable(temp.head->elem)));
            }
            else
                if (temp.head->lexeme.lexToken._eToken==token_str)
                    program.putLexem(Lexeme(token_str,temp.head->elem));
                else
                    program.putLexem(Lexeme(token_number,temp.head->numElem));
            next_lex(temp);
        }
    }while(temp.head->lexeme.lexToken._eToken==token_semicolon);
    if ((temp.head->lexeme.lexToken._tToken==token_eol)||(temp.head->lexeme.lexToken._tToken==token_eof)) {
        program.putLexem(token_write);
        return ;
    }
    strcpy(err.msg, "Print Function");
    throw err;
}

void Parser::arr(List &temp){
    next_lex(temp);
    int vsize=0,index=0;
    char* type = new char[128];
    if (temp.head->lexeme.lexToken._tToken==token_identifier) {
        index=checkTable(temp.head->elem);
        next_lex(temp);
        if (temp.head->lexeme.lexToken._eToken==token_lb) {
            next_lex(temp);
            if (temp.head->lexeme.lexToken._tToken==token_number) {
                vsize=temp.head->numElem;
                next_lex(temp);
                if (temp.head->lexeme.lexToken._eToken==token_rb) {
                    next_lex(temp);
                    if ((temp.head->lexeme.lexToken._eToken==token_integer) ||(temp.head->lexeme.lexToken._tToken==token_string)){
                        strcpy(type, temp.head->elem);
                        updateTable(vsize, index, type);
                        next_lex(temp);
                        return ;
                    }
                }
            }
        }
    }
    strcpy(err.msg, "Array");
    throw err;
}

void Parser::whilecycle(List &temp){
    int line1,line2;
    line1 = program.getFree();
    next_lex(temp);
    expression0(temp);
    next_lex(temp);
    line2 = program.getFree();
    program.blank();
    program.putLexem(Lexeme(poliz_fgo));
    while ((temp.head->tokElem!=token_wend) && (temp.head->lexeme.lexToken._tToken!=token_eof)) {
        sub_analyze(temp);
    }
    if (temp.head->tokElem == token_wend) {
        next_lex(temp);
        program.putLexem(Lexeme(poliz_label,line1));
        program.putLexem(poliz_go);
        program.putLexem(Lexeme(poliz_label,program.getFree()),line2);
        return;
    }
    strcpy(err.msg, "While Cycle");
    throw err;
    
}
void Parser::forcycle(List & temp){
    int line1,line2;
    line1 = program.getFree();
    next_lex(temp);
    
    ident(temp);
    if (temp.head->lexeme.lexToken._eToken == token_to) {
        next_lex(temp);
        if((temp.head->lexeme.lexToken._tToken == token_number)||
           (temp.head->lexeme.lexToken._tToken == token_identifier)){
            program.putLexem(temp.head->lexeme.lexToken._eToken);
            next_lex(temp);
            line2 = program.getFree();
            program.blank();
            program.putLexem(poliz_fgo);
            while ((temp.head->lexeme.lexToken._eToken!=token_next) && (temp.head->lexeme.lexToken._tToken!=token_eof)) {
                sub_analyze(temp);
            }
            if (temp.head->lexeme.lexToken._eToken == token_next) {
                next_lex(temp);
                program.putLexem(Lexeme(poliz_label,line1));
                program.putLexem(poliz_go);
                program.putLexem(Lexeme(poliz_label,program.getFree()),line2);
                return;
            }
        }
    }
    
    strcpy(err.msg, "For Cycle");
    throw err;
}

void Parser::expression0(List & temp){
    expression1(temp);
    if (temp.head->lexeme.lexToken._eToken == token_assign ||
        temp.head->lexeme.lexToken._eToken == token_less ||
        temp.head->lexeme.lexToken._eToken == token_more)
    {
        tokens.push(temp.head->lexeme.lexToken._eToken);

        next_lex(temp);
        expression1(temp);
        checkOperators();
        
    }
}

void Parser::expression1(List& temp){
    expressionHigh(temp);
    while ( temp.head->lexeme.lexToken._eToken == token_add ||
           temp.head->lexeme.lexToken._eToken == token_sub ||
           temp.head->lexeme.lexToken._eToken == token_or)
    {
        tokens.push(temp.head->lexeme.lexToken._eToken);
        next_lex(temp);
        expressionHigh(temp);
        checkOperators();
    }
}

void Parser::expression2(List& temp){
    if ( temp.head->lexeme.lexToken._tToken == token_identifier )
    {
        tokens.push(getType(temp.head->elem));
        
        program.putLexem(Lexeme(token_identifier,checkTable(temp.head->elem)));
        next_lex(temp);
        //checkNextBr(temp,poliz_arrayindexvalue);
    }
    else if ( temp.head->lexeme.lexToken._tToken == token_number )
    {
        tokens.push(token_integer);
        program.putLexem(Lexem(temp.head->lexeme.lexToken._eToken,temp.head->numElem));
        next_lex(temp);
    }
    else if(temp.head->lexeme.lexToken._eToken == token_str)
    {
        tokens.push(token_string);
        program.putLexem(Lexem(temp.head->lexeme.lexToken._eToken,temp.head->elem));
        next_lex(temp);
    }
    else if (temp.head->lexeme.lexToken._eToken == token_not)
    {
        next_lex(temp);
        expression2(temp);
    }
    else if ( temp.head->lexeme.lexToken._eToken == token_lb )
    {
        next_lex(temp);
        expression0(temp);
        if ( temp.head->lexeme.lexToken._eToken == token_rb)
            next_lex(temp);
        else
            throw err;
    }
    else
        throw err;
}

void Parser::expressionHigh(List& temp){
    expression2(temp);
    while (temp.head->lexeme.lexToken._eToken == token_mul ||
           temp.head->lexeme.lexToken._eToken == token_div ||
           temp.head->lexeme.lexToken._eToken == token_and)
    {
        tokens.push(temp.head->lexeme.lexToken._eToken);
        next_lex(temp);
        expression2(temp);
        checkOperators();
    }
}

int Parser::checkTable(char* name){
    for (int i = 0;i<tableSize;i++ ) {
        if (!strcmp(name,tableID[i].getName())){
            //IDs.push(i);
            
            return i;
        }
    }
    return -1;
}

void Parser::checkOperators(){
    Token Var1,Var2,Op;
    if ((tokens.top() == token_and)||(tokens.top() == token_or)) {
        tokens.pop();
        return ;
    }
    Var1 = tokens.top();
    tokens.pop();
    Op = tokens.top();
    tokens.pop();
    Var2 = tokens.top();
    tokens.pop();
    
    if(Op!=token_assign)
        tokens.push(token_integer);
    if ((Op==token_assign)&& (ifoperator)){
        program.putLexem(poliz_eq);
    }
    else{
        program.putLexem(Op);
    }
}

Token Parser::getType(char* name){
    id temp;
    char* tempname = new char [128];
    for (int i = 0;i<tableSize;i++ ) {
        strcpy(tempname,tableID[i].getName());
        if (!strcmp(name,tempname)){
            temp=tableID[i];
            break;
        }
    }
    delete[] tempname;
    if (temp.type == basicInt) {
        return token_integer;
    }
    else
        return token_str;
}
void Parser::copyIDs(stack<int> &temp){
    temp=IDs;
}
void Parser::copyTokens(stack<Token> &temp){
    temp=tokens;
}

void Parser::updateTable(int newSize,int index, char* newType){
    if(!strcmp(newType, "STRING"))
        tableID[index].type=basicStr;
    else
        tableID[index].type=basicInt;
    tableID[index].data.resize(newSize);
}
void Parser::showTable(){
    cout<<endl;
    cout<<"Updated Table Of Names"<<endl;
    for (int i = 0; i<tableSize; i++) {
        cout<<i+1<<'\t'<<tableID[i].getName()<<'\t'<<"Has "<<tableID[i].data.size()<<" Element(s) \t";
        if (tableID[i].getType()==basicInt)
            cout<<"Type: Int"<<endl;
        else
            cout<<"Type: String"<<endl;
    }
}
void Parser::cpyTable(id* Tbl){
    for (int i=0;i<tableSize;i++){
        //for(int k=0;k<tableID[i].data.size();k++){
        //Tbl[i].data[k]=tableID[i].data[k];
        memcpy(&Tbl[i].data,&tableID[i].data,sizeof(vector<set>)*tableID[i].data.size());
        //}
        
        Tbl[i].type=tableID[i].type;
        Tbl[i].value=tableID[i].value;
        strcpy(Tbl[i].name,tableID[i].name);
        
    }
}
