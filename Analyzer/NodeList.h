//
//  MyList.h
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#ifndef __task3__NodeList__
#define __task3__NodeList__

#include <string.h>
#include "definition.h"

using namespace std;

struct Node{
    Node *next;
    Lexeme lexeme;
    char* elem;
    int numElem;
    
    Node(char *NewElem, int newNum, Lexeme newLex){
        strcpy(elem,NewElem);
        numElem=newNum;
        lexeme=newLex;
        next = NULL;
    }
    Node(){
        elem=new char[128];
        numElem=0;
        next=NULL;
    }
    Node(Node& node){
        elem=node.elem;
        numElem=node.numElem;
        lexeme=node.lexeme;
        next=node.next;
    }
    operator Lexeme(){
        Lexeme lex;
        lex.lexToken=lexeme.lexToken;
        lex.lexNumber=numElem;
        return lex;
    }
    enumToken GetNodeToken(){
        return lexeme.lexToken._eToken;
    }
};

class List{
    int nodes;
    
public:
    Node *head;
    List();
    List(const List&);
    
    void AddNode();
    void AddNode(Lexeme lexeme);
    
    enumToken getToken();
    void PrintList();
    List& operator=(const List&);
    ~List();
};

#endif /* defined(__task3__MyList__) */