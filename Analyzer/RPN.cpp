//
//  RPN.cpp
//  task3
//
//  Created by Дмитрий Кузнецов on 26.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#include "RPN.h"

RPN::RPN(int max_size){
    p = new Lexeme [size = max_size];
    free = 0;
};
void RPN::putLexem(Lexeme l){
    p[free] = l.lexToken;
    ++free;
}
void RPN::blank(){
    ++free;
}
void RPN::putLexem(Lexeme l, int place){
    p[place] = l;
}
int RPN::getFree() {
    return free;
};
Lexeme& RPN::operator[](int index){
    if (index > size) {
        size_t newSize = size*2;
        Lexeme* newArr = new Lexeme[newSize];
        memcpy(newArr,p,size*sizeof(Lexeme));
        size=(int)newSize;
        delete[] p;
        p = newArr;
        throw "In RPN: indefinite element of array";
    }
    else
        if (index > free)
            throw "In RPN: indefinite element of array";
        else
            return p[index];
}
void RPN::print(){
    cout<<endl;
    cout<<"RPN"<<endl;
    for (int i = 0; i < free; ++i )
        cout << i <<'\t'<< p[i];
}
RPN::~RPN(){
    delete []p;
}