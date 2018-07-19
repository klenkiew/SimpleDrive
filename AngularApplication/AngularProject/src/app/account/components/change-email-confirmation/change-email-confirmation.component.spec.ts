import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangeEmailConfirmationComponent } from './change-email-confirmation.component';

describe('ChangeEmailConfirmationComponent', () => {
  let component: ChangeEmailConfirmationComponent;
  let fixture: ComponentFixture<ChangeEmailConfirmationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChangeEmailConfirmationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangeEmailConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
