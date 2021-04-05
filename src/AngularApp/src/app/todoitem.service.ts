import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable, of } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';

import { TodoItem } from './todoItem';
import { MessageService } from './message.service';


@Injectable({ providedIn: 'root' })
export class TodoItemService {

  private todoItemsUrl = '/api/todoitems';  // URL to web api

  httpOptions = {
    headers: new HttpHeaders({ 'Content-Type': 'application/json' })
  };

  constructor(
    private http: HttpClient,
    private messageService: MessageService) { }

  /** GET todoItems from the server */
  getTodoItems(): Observable<TodoItem[]> {
    return this.http.get<TodoItem[]>(this.todoItemsUrl)
      .pipe(
        tap(_ => this.log('fetched todoItems')),
        catchError(this.handleError<TodoItem[]>('getTodoItems', []))
      );
  }

  /** GET todoItem by id. Return `undefined` when id not found */
  getTodoItemNo404<Data>(id: number): Observable<TodoItem> {
    const url = `${this.todoItemsUrl}/?id=${id}`;
    return this.http.get<TodoItem[]>(url)
      .pipe(
        map(todoItems => todoItems[0]), // returns a {0|1} element array
        tap(h => {
          const outcome = h ? `fetched` : `did not find`;
          this.log(`${outcome} todoItem id=${id}`);
        }),
        catchError(this.handleError<TodoItem>(`getTodoItem id=${id}`))
      );
  }

  /** GET todoItem by id. Will 404 if id not found */
  getTodoItem(id: number): Observable<TodoItem> {
    const url = `${this.todoItemsUrl}/${id}`;
    return this.http.get<TodoItem>(url).pipe(
      tap(_ => this.log(`fetched todoItem id=${id}`)),
      catchError(this.handleError<TodoItem>(`getTodoItem id=${id}`))
    );
  }

  /* GET todoItems whose name contains search term */
  searchTodoItems(term: string): Observable<TodoItem[]> {
    if (!term.trim()) {
      // if not search term, return empty todoItem array.
      return of([]);
    }
    return this.http.get<TodoItem[]>(`${this.todoItemsUrl}/?name=${term}`).pipe(
      tap(x => x.length ?
         this.log(`found todoItems matching "${term}"`) :
         this.log(`no todoItems matching "${term}"`)),
      catchError(this.handleError<TodoItem[]>('searchTodoItems', []))
    );
  }

  //////// Save methods //////////

  /** POST: add a new todoItem to the server */
  addTodoItem(todoItem: TodoItem): Observable<TodoItem> {
    return this.http.post<TodoItem>(this.todoItemsUrl, todoItem, this.httpOptions).pipe(
      tap((newTodoItem: TodoItem) => this.log(`added todoItem w/ id=${newTodoItem.id}`)),
      catchError(this.handleError<TodoItem>('addTodoItem'))
    );
  }

  /** DELETE: delete the todoItem from the server */
  deleteTodoItem(id: number): Observable<TodoItem> {
    const url = `${this.todoItemsUrl}/${id}`;

    return this.http.delete<TodoItem>(url, this.httpOptions).pipe(
      tap(_ => this.log(`deleted todoItem id=${id}`)),
      catchError(this.handleError<TodoItem>('deleteTodoItem'))
    );
  }

  /** PUT: update the todoItem on the server */
  updateTodoItem(todoItem: TodoItem): Observable<any> {
    return this.http.put(this.todoItemsUrl, todoItem, this.httpOptions).pipe(
      tap(_ => this.log(`updated todoItem id=${todoItem.id}`)),
      catchError(this.handleError<any>('updateTodoItem'))
    );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {

      // TODO: send the error to remote logging infrastructure
      console.error(error); // log to console instead

      // TODO: better job of transforming error for user consumption
      this.log(`${operation} failed: ${error.message}`);

      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }

  /** Log a TodoItemService message with the MessageService */
  private log(message: string) {
    this.messageService.add(`TodoItemService: ${message}`);
  }
}
