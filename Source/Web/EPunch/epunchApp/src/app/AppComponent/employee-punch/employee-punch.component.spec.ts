import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeePunchComponent } from './employee-punch.component';

describe('EmployeePunchComponent', () => {
  let component: EmployeePunchComponent;
  let fixture: ComponentFixture<EmployeePunchComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ EmployeePunchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(EmployeePunchComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
