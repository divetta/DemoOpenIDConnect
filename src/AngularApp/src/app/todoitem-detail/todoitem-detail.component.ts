import { TodoItem } from './../todoitem';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-todoitem-detail',
  templateUrl: './todoitem-detail.component.html',
  styleUrls: ['./todoitem-detail.component.css']
})
export class TodoitemDetailComponent implements OnInit {
  @Input() todoItem?: TodoItem;

  constructor() { }

  ngOnInit() {
  }

}
