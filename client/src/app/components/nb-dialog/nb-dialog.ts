import { Component, ElementRef, input, output, viewChild } from '@angular/core';

@Component({
  selector: 'app-nb-dialog',
  imports: [],
  templateUrl: './nb-dialog.html',
  styleUrl: './nb-dialog.scss',
})
export class NbDialog {

  title = input.required<string>();
  closed = output();
  dialogRef = viewChild<ElementRef<HTMLDialogElement>>('dialogRef');


  open(): void {
    this.dialogRef()?.nativeElement.show();
  }

  close(): void {
    this.dialogRef()?.nativeElement.close();
    this.closed.emit();
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target === this.dialogRef()?.nativeElement) {
      this.close();
    }
  }
}
