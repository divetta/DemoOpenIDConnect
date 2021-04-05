

import { Component, OnInit } from '@angular/core';

import { TodoItem } from './../todoitem';
import { TodoItemService } from './../todoitem.service';
import { MessageService } from './../message.service';

@Component({
  selector: 'app-todoitem',
  templateUrl: './todoitem.component.html',
  styleUrls: ['./todoitem.component.css']
})
export class TodoitemComponent implements OnInit {

  selectedTodoItem?: TodoItem;

  todoItems: TodoItem[] = [];

  constructor(private todoItemService: TodoItemService, private messageService: MessageService) { }

  ngOnInit() {
    this.getTodoItems();
  }

  onSelect(todoItem: TodoItem): void {
    this.selectedTodoItem = todoItem;
    this.messageService.add(`TodoItemsComponent: Selected TodoItem id=${todoItem.id}`);
  }

  getTodoItems(): void {
    this.todoItemService.getTodoItems()
        .subscribe(todoItems => this.todoItems = todoItems);
  }

}
