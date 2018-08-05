import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ProtectedAreaComponent } from './protected-area.component';

describe('ProtectedAreaComponent', () => {
  let component: ProtectedAreaComponent;
  let fixture: ComponentFixture<ProtectedAreaComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ProtectedAreaComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ProtectedAreaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
