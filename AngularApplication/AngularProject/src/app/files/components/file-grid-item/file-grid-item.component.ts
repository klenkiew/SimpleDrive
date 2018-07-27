import {Component, EventEmitter, HostListener, Input, OnInit, Output} from '@angular/core';
import {File} from "../../../shared/models/file";
import {ContextMenu} from "primeng/primeng";

@Component({
  selector: 'app-file-grid-item',
  templateUrl: './file-grid-item.component.html',
  styleUrls: ['./file-grid-item.component.css']
})
export class FileGridItemComponent implements OnInit {

  @Input() file: File;
  @Input() contextMenu: ContextMenu;
  @Input() icon: string = 'fa-file';

  @Output() fileSelected: EventEmitter<File> = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  @HostListener('contextmenu', ['$event'])
  onContextMenu(event: MouseEvent): void {
    event.preventDefault();
    this.contextMenu && this.contextMenu.show(event);
  }
}
