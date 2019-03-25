//
//  Parser.h
//  task3
//
//  Created by Дмитрий Кузнецов on 26.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#ifndef __task3__Parser__
#define __task3__Parser__

#include <iostream>
#include "NodeList.h"
#include "ID.h"
#include "RPN.h"
#include <string.h>
#include "Exceptions.h"
#include <stack>

class Parser{
    List list;
    stack<enumToken> tokens;
    stack<int> IDs;
    Error err;
    ID* tableID;
    size_t tableSize;
    int count;
    bool ifoperator;
    
    void next_lex(List&);
    void sub_analyze(List&);
    void ifoper(List&);
    void inputfunction(List&);
    void outputfunction(List&);
    void forcycle(List&);
    void whilecycle(List&);
    void ident(List&);
    void arr(List&);
    void checkRedeclaration();
    void checkNextBr(List&, Lexeme);
    void expression0(List&);
    void expression1(List&);
    void expression2(List&);
    void expressionHigh(List&);
    int checkTable(char*);
    void checkOperators();
    enumToken getType(char*);
    void copyIDs(stack<int>&);
    void copyTokens(stack<enumToken>&);
    void updateTable(int newSize,int index, char* newType);
    void showTable();
    void cpyTable(ID* TBL);
    
public:
    RPN program;
    Parser(List&,ID*,size_t);
    int analyze();
    ~Parser(){};
};

#endif /* defined(__task3__Parser__) */
