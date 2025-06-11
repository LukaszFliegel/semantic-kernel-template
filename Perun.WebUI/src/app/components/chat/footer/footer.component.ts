import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './footer.component.html',
  styleUrl: './footer.component.scss'
})
export class FooterComponent {
  @Input({ required: true }) isDisabled: boolean = false;


  @Output() attachClicked = new EventEmitter<File>();
  @Output() sendClicked = new EventEmitter<string>();

  @ViewChild('fileInput') fileInput!: ElementRef;

  file!: File | null;

  inputForm: FormGroup;

  constructor(private fb: FormBuilder) {
    this.inputForm = this.fb.group({
      text: [null, [Validators.maxLength(100)]]
    });
  }

  triggerFileSelect(): void {
    this.fileInput.nativeElement.click();
  }

  attach(event: any) {
    const file = event.target.files[0];

    this.file = file;
    this.attachClicked.emit(file);
  }

  send() {
    this.sendClicked.emit(this.inputForm.controls['text'].value);

    this.file = null
    this.inputForm.reset();
  }
}
