//
//  RPN.h
//  task3
//
//  Created by Дмитрий Кузнецов on 26.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#ifndef __task3__RPN__
#define __task3__RPN__

#include <iostream>
#include "NodeList.h"
class RPN{
    Lexeme *p;
    int size;
    int free;
public:
    RPN (int max_size);
    void putLexem(Lexeme l);
    void putLexem(Lexeme l, int place);
    void blank();
    int getFree();
    Lexeme& operator[] (int index);
    void print();
    ~RPN();
    
};

#endif /* defined(__task3__RPN__) */
