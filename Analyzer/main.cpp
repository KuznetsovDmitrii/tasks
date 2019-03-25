//
//  main.cpp
//  task3
//
//  Created by Дмитрий Кузнецов on 13.04.15.
//  Copyright (c) 2015 MSU. All rights reserved.
//

#include <iostream>
#include <cstdio>
#include <cstdlib>
#include "NodeList.h"
#include "Analyzer.h"
#include "ID.h"

#define READ_INTERVAL 16

int main(int argc, const char** argv) {
    Analyzer analyzer;
    
    char* program = NULL;
    int sym,i=0;
    long scan_len=READ_INTERVAL;
    program = (char*) calloc(scan_len, sizeof(char));
    FILE *f = stdin;
    
    if ((argc>1) && (argv[1]!=NULL)){
        f = fopen(argv[1],"r");
        if (f==NULL){
            cout<<"Error by openning file: "<<argv[1]<<endl;
            exit(0);
        }
    }
    do{
        sym=fgetc(f);
        program[i++]=(char)sym;
        
        if(i>=scan_len){
            scan_len+=READ_INTERVAL;
            program=(char*) realloc(program,scan_len*sizeof(char));
        }
    }
    while(sym!=EOF);
    if (argc>1 && argv[1]!=NULL){
        fclose(f);
    }
    
    analyzer.GetLexemes(program);
    analyzer.ShowLexemes();
    analyzer.ShowTable();
    
    free(program);
    return 0;
}


