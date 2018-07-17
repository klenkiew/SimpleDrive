import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FileGridItemComponent } from './file-grid-item.component';

describe('FileGridItemComponent', () => {
  let component: FileGridItemComponent;
  let fixture: ComponentFixture<FileGridItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FileGridItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FileGridItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
