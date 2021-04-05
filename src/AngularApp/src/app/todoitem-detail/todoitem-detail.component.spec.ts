import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TodoitemDetailComponent } from './todoitem-detail.component';

describe('TodoitemDetailComponent', () => {
  let component: TodoitemDetailComponent;
  let fixture: ComponentFixture<TodoitemDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TodoitemDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TodoitemDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
