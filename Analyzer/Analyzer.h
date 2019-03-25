//
//  Analizator.h
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#ifndef __task3__Analizer__
#define __task3__Analizer__

#include <iostream>
#include <ctype.h>
#include "NodeList.h"
#include "Id.h"

class Analyzer{
private:
    ID *IDTable;
    int SizeOfIDTable;
    
    List LexemeList;
    
public:
    Analyzer();
    ~Analyzer();
    
    int GetSize();
    int GetLex(char *program, long& pos);
    void GetLexemes(char *program);
    void AddId(char *name);
    void ShowTable();
    void ShowLexemes();
    void CopyList(List& list);
    void CopyTable(ID *table);
};

#endif /* defined(__task3__Analizer__) */