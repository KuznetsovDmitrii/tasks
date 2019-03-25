//
//  MyList.cpp
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#include "NodeList.h"

const char *TokenTypeString[] = {
    "Error",
    "Keyword",
    "Sign of Operation",
    "Delaration",
    "Number",
    "String",
    "Identifier",
    "Comment",
    "End of Line",
    "End of File"
};

List::List(){
    nodes=0;
    head=NULL;
}
List::List(const List& temp){
    *this=temp;
}

void List::AddNode(Lexeme lexeme){
    Node *temp = head;
    if (head){
        while (temp->next)
            temp=temp->next;
        temp->next = new Node;
        temp=temp->next;
    }
    else{
        head = new Node;
        temp = head;
    }
    temp->lexeme=lexeme;
    temp->next=NULL;
    nodes++;
}

void List::AddNode(){
    Node *temp = head;
    if (head){
        while (temp->next)
            temp=temp->next;
        temp->next = new Node;
        temp=temp->next;
    }
    else{
        head = new Node;
        temp = head;
    }
    temp->next=NULL;
    nodes++;
}

void List::PrintList(){
    cout<<endl<<"List of lexemes"<<endl<<endl;
    Node *node = head;
    for (;node;node=node->next){
        cout<<"Lexeme type"<<endl<<"\t"<<TokenTypeString[node->lexeme.lexToken._tToken];
        if (node->lexeme.lexToken._putToken!=NULL && strlen(node->lexeme.lexToken._putToken)>0){
            cout<<": " <<node->lexeme.lexToken._putToken;
        }
       // cout<<endl;
        if (node->lexeme.lexString!=NULL && strlen(node->lexeme.lexString)>0){
            cout<<" "<<node->lexeme.lexString;
        }
        cout<<endl;
    }
}

List& List::operator=(const List &list){
    Node *node = list.head;
    for (;node;node=node->next){
        AddNode(node->lexeme);
    }
    return *this;
}

List::~List(){
    Node *node;
    while (head){
        node=head;
        head=head->next;
        free(node);
    }
}

enumToken List::getToken(){
    return head->GetNodeToken();
}