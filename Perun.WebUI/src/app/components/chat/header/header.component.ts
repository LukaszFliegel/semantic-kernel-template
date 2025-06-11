import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output, signal } from '@angular/core';

@Component({
  selector: 'app-header',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  @Input({ required: true }) text = '';

  @Output() closeClicked = new EventEmitter<void>();

  close() {
    this.closeClicked.emit();
  }
}
