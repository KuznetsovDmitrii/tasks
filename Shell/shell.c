//
//  main.c
//  Shell
//
//  Created by Кузнецов Дмитрий on 22.07.14.
//  Copyright (c) 2014 Dmitry Kuznetsov. All rights reserved.
//
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <errno.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/wait.h>
#include <signal.h>
#include <fcntl.h>

#define EndOfFile -1
#define EndOfLine -2
#define Permission 0666

typedef struct S{
    char *word;
    struct S *next;
} List;

char home_directory[1024];
int ampersant = 0;
char **stream_in=NULL, **stream_out=NULL, **stream_add=NULL;

void my_free(List *list){
	List *tmp;
	tmp=list;
    free(list->word);
	list=list->next;
	free(tmp);
	while (list!=NULL){
		tmp=list;
        free(list->word);
		list=list->next;
		free(tmp);
	}
}

List *my_alloc(void){
    List *list = (List*)malloc(sizeof(List));
    memset(list, 0, sizeof(List));
    return list;
}

void strucprint(List *list){
	while (list!=NULL){
		if (list->word!=NULL)
            printf("#%s#\n",list->word);
		list=list->next;
	}
}

int Node_Count(List *list){
	int i=0;
	while (list){
		list=list->next;
		i++;
	}
	return i;
}

int Pipe_Count(List *list){
	int count=0;
	while (list){
		if (strcmp(list->word,"|")==0)
			count++;
		list=list->next;
	}
	return count;
}

List *my_add(List *list, char *string){
    List *tmp = my_alloc();
    list->next = tmp;
    tmp->word = string;
    tmp->next = NULL;
    return tmp;
}

int cheking_stream(List *list){
	if ((strcmp(list->next->word,">")==0) || ((strcmp(list->next->word,"<")==0))){
		printf("Wrong redirection\n");
		return -1;
	}
	if (strcmp(list->next->word,">>")==0){
		printf("Wrong redirection\n");
		return -1;
	}
    return 0;
}

char **Node_to_Vector(List *list, int *error){
	char **q, **tmp=malloc((Node_Count(list)+1)*sizeof(*tmp));
    int in=0, count, erno;
    count=Pipe_Count(list);
    stream_in=(char**)calloc(100,sizeof(char*));
    stream_out=(char**)calloc(100,sizeof(char*));
    stream_add=(char**)calloc(100,sizeof(char*));
    q=tmp;
	while (list){
        if (strcmp(list->word,"|")==0)
            in++;
        
        if (strcmp(list->word,"<") == 0){
            erno = cheking_stream(list);
            if (erno == 0){
                    stream_in[in] = strdup(list->next->word);
                    list = list->next->next;
            }
            else 
                return NULL;
        }
        else 
            if (strcmp(list->word,">") == 0){
                erno = cheking_stream(list);
                if (erno == 0){
                        stream_out[in] = strdup(list->next->word);
                        list = list->next->next;
                }
                else
                    return NULL;
            }
            else
                if (strcmp(list->word,">>") == 0){
                    erno = cheking_stream(list);
                    if (erno == 0){
                            stream_add[in] = strdup(list->next->word);
                            list = list->next->next;
                    }
                    else 
                        return NULL;
                }
        
        else{
    		*q=list->word;
    		list=list->next;
    		q++;
        }
	}
	*q=NULL;
	return tmp;	
}

char **Node2Vector(char **list, int *k, List *lisst){
	char **q,**tmp=malloc((Node_Count(lisst))*sizeof(*tmp));
	q=tmp;
	while (list[*k]!=NULL){
        if (strcmp(list[*k],"|")==0){
            *k+=1;
            break;
        }
        *q=list[*k];
        q++;
        *k+=1;
	}
	*q=NULL;
	return tmp;
}

char ***PipeNode2Str(char ***argvv, List *list, char **qwe){
    int k=0,i=0,error=0;
    char **str, ***q, ***tmp=malloc((Pipe_Count(list))*sizeof(**tmp));
    q=tmp;
    qwe=Node_to_Vector(list, &error);
	if (strcmp(qwe[0],"|")==0){
		printf("I didn't expect '|' here!\n");
        return NULL;
	}
	if (error==-1){
		printf("ERROR: Invalid i/o redirection use!\n");
		return NULL;
	}
	if (error==-2){
		printf("ERROR:Double i/o redirection!\n");
		return NULL;
	}
	while (qwe[k]){
        if ((str=Node2Vector(qwe,&k,list))!=NULL){
            *q=str;
            q++;
            i++;
        }
	}
    *q=NULL;
	return tmp;  
}

int cd(char **argvv){
    if ((argvv[1] != NULL) && (argvv[2] != NULL))
        return -2;
	if (argvv[1] != NULL){
		if ((chdir(argvv[1]))==-1){
            perror("Chdir 1");
			//printf("Unable to locate the work directory: %s\n", argvv[1]);
		}
	}
	else
	{
		if (chdir(home_directory)==-1){
			//printf("Unable to locate the home directory: %s\n", home_directory);
            perror("Chdir 2");
		}
	}
	return 0;
}

void stream(char **in, char **out, char **add){
    int file1, file2, file3;
    if (in[0] != NULL){
        if ((file1 = open(in[0],O_RDONLY))==-1){
            perror("STREAM_IN");
            _exit(2);
        }
        dup2(file1,0);
        close(file1);
    }
    if (out[0] != NULL){
        if ((file2 = open(out[0],O_WRONLY|O_CREAT|O_TRUNC,Permission))==-1){
            perror("STREAM_OUT");
            _exit(2);
        }
        dup2(file2,1);
        close(file2);
    }
    if (add[0] != NULL){
        if ((file3 = open(add[0],O_WRONLY|O_CREAT|O_APPEND,Permission))==-1){
            perror("STREAM_ADD");
            _exit(2);
        }
        dup2(file3,1);
        close(file3);
    }
}

void pipeline(char ***argvv,int num_command,char **in,char **out,char **add,int PC,int status){
    int file1, file2, file3;
    int number=PC+1, fd[PC][2];
    pid_t pipe_pid;
    if (argvv[1]==NULL){
        printf("Incorrect use of '|' !\n");
        return;
    }

    for (num_command=0; num_command<number; num_command++){
        if (num_command!=number-1)
            pipe(fd[num_command]);
        if((pipe_pid=fork())){
            if(pipe_pid ==  -1){
                perror("PIPE_FORK");
                exit(4);
            }
            if (num_command!=number-1){
                close(fd[num_command][1]);
            }
            if (!ampersant)
                waitpid(pipe_pid, &status, 0);
        }
        else{
                if (stream_in[num_command]!=NULL){
                    if ((file1=open(stream_in[num_command], O_RDONLY, 0))==-1){
                        perror("PIPE_STREAM_IN");
                        exit(3);
                    }
                    dup2(file1,0);
                    close(file1);
                }
                else{
                    if (num_command)
                        dup2(fd[num_command-1][0],0);    
                }
                
                if (stream_out[num_command]!=NULL){
                    if ((file2=open(stream_out[num_command], O_WRONLY|O_CREAT|O_TRUNC, 0666))==-1){
                        perror("PIPE_STREAM_OUT");
                        exit(3);
                    }
                    dup2(file2,1);
                    close(file2);
                }
                else
                    if (stream_add[num_command]!=NULL){
                        if ((file3=open(stream_add[num_command], O_WRONLY|O_CREAT|O_APPEND, 0666))==-1){
                            perror("PIPE_STREAM_ADD");
                            exit(3);
                        }
                        dup2(file3,1);
                        close(file3);
                    }
                    else{
                        if (num_command!=number-1)
                            dup2(fd[num_command][1],1);
                    }
      
            execvp(argvv[num_command][0], argvv[num_command]);
            perror(argvv[num_command][0]);
            exit(4);
            return;
        }
    }
}

void command(char ***argvv, char **in, char **out, char **add, int PC){
    pid_t p_pid, ppid;
	int status = 1, num_command = 0;
    if (PC == 0){
    	if ((p_pid = fork())){
    		if (p_pid == -1){
    			perror("FORK");
    			return;
    		}
            printf("Process with ID %i is running\n",p_pid);
            if (!ampersant){
                waitpid(p_pid,&status,0);
    		    if (WIFSIGNALED(status))
    			    printf("Process ID %d terminated with singal # %i\n",p_pid,WTERMSIG(status));
    		    else
    			    if (WIFEXITED(status)!=0)
    				    printf("Process ID %d terminated with code %i\n",p_pid,WEXITSTATUS(status));
            }
        
    		while ((ppid=waitpid(-1,&status,WNOHANG))>0) {
    			if (WIFSIGNALED(status))
    				printf("Process ID %d terminated with singal # %i\n",ppid,WTERMSIG(status));
    			else
    				if (WIFEXITED(status)!=0)
    					printf("Process ID %d terminated with code %i\n",ppid,WEXITSTATUS(status));
    		}
        
    	}    
	    else{
            stream(in,out,add);
    		execvp(argvv[0][0],argvv[0]);
    		perror("EXEC");
    		exit(1);
    	}
    }
    else{
        pipeline(argvv,num_command,in,out,add,PC,status);
        return;
    }
}

void ParseCom(char ***argvv, int PC){
	int erno = 0;
	if (strcmp(argvv[0][0],"cd") == 0){
		if ((erno=cd(argvv[0])) == -2)
			printf("Uncorrect parametrs for cd\n");
		return;
	}
	else {
		command(argvv, stream_in, stream_out, stream_add, PC);
		return;
	}
}

char *my_reader(int *symbol, int *bad_ampersant){
    int size = 128, flag = 0, count = 0;
    int a = getchar();
    if (a == '\n'){
        *symbol = EndOfLine;
        return NULL;
    }
    char *string = (char *)calloc(size, sizeof(char));
    while (((a != EndOfFile) && (a != '\n')) || (flag == 1)) {
        if (((a == '|') && (flag != 1)) 
                || ((a == '<') && (flag != 1))){
            if (count > 0){
                *(string+count) = '\0';
                ungetc(a,stdin);
                return string;
            }
            else {
                *string = a; 
                *(string + 1) = '\0';
                return string;
            }
        }
        if ((a == '>') && (flag != 1)){
				if ((a=getchar())=='>'){
					*string='>';
					*(string+1)=a;
					*(string+2)='\0';
					return string;
				} 
				else {
					ungetc(a,stdin);
					*string='>';
					string[1]='\0';
					return string;
				}
        }
        if (a == '"')
			flag = 1-flag;
        if ((a == ' ') || (a == '\n') || (a == '\t')){
            if (flag){
                *(string+count) = a;
                ++count;
                if (count == size){
                    size += size;
                    string = (char *)realloc(string, size);
                }
            }
            else
                break;
        }
        if (a == '&'){
            ampersant = 1;
            do{
                a = getchar();
                if ((a != ' ') && (a != '\t') && (a != '\n') && (a != EndOfFile)){
                    *bad_ampersant = 1;
                    fprintf(stderr,"Incorrect using of &\n");
                  //  exit(-2);
                    break;
                    
                }
            }while ((a != '\n') && (a != EndOfFile) && (*bad_ampersant != 1));
                break;
        }     
        if ((a != '"') && (a != '\n') && (a != ' ') && (a != '\t')){
            *(string+count) = a;
            ++count;
            if (count == size){
                size += size;
                string = (char *)realloc(string, size);
            }
        }  
        a = getchar();
    }
    
    *(string+count) = '\0';
	*symbol = 0;
	if (a == EOF)
        *symbol = EndOfFile;
    if (a == '\n')
        *symbol = EndOfLine;
    if (count > 0)
        return string;
    else
        return NULL;
}

int kill_zombie(){
    pid_t pid;
    int status;
    if ((pid=wait4(-1,&status,WNOHANG,NULL))>0){
    	printf("Process ID %i terminated",pid);
    	if (WIFEXITED(status)!=0)
    	    printf(" with code %i\n",WEXITSTATUS(status));
    }
    return 0;
} 
 
int starter(){
    char **qwe = NULL;
    int bad_ampersant = 0;
    int PC = 0,count = 0, symbol = 0;
    char work_directory[1024], *slovo;
    char ***argvv;
    List *node = my_alloc(), *head = my_alloc();
    head = node;
    node->next = NULL;
    
    while (symbol != EndOfFile){
		getcwd(work_directory,sizeof(work_directory));
        kill_zombie();
		printf("@Dmitry@:%s:",strrchr(work_directory,'/')+1);
		symbol = 0;
        count = 0;
        while ((symbol != EndOfFile) && (symbol != EndOfLine)){
            slovo = my_reader(&symbol, &bad_ampersant);
    		if (!slovo)
    			continue;
            if (strcmp(slovo,"\n") != 0){
                if (count == 0){
                    node->word = strdup(slovo);
                    node->next = NULL;
                    ++count;
                }
                else{
                    node->next = my_add(node, slovo);
                    node = node->next;
                    ++count;
                }
            }
        }
    	if (symbol == EndOfFile){
            printf("\n");
            break;
        }
    	if ((slovo != NULL) || ((slovo == NULL) && (count > 0))){
            if (bad_ampersant == 0){;
                //strucprint(head);
                PC=Pipe_Count(head);
                argvv=PipeNode2Str(argvv,head,qwe);
                if(argvv)
                    ParseCom(argvv,PC);
            }
            free(qwe);
    		my_free(head);
            bad_ampersant = 0;
            ampersant = 0;
            stream_in = stream_out = stream_add = NULL;
            
            
            node=my_alloc();
            head=node;
            node->word=NULL;
            node->next=NULL;
        }
    }
    return 0;
}

int main(int argc, char **argv){
	getcwd(home_directory,sizeof(home_directory));
	starter();
	return 0;
}