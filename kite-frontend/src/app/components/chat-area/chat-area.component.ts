import { Component, ElementRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-chat-area',
  imports: [],
  templateUrl: './chat-area.component.html',
  styleUrl: './chat-area.component.css'
})
export class ChatAreaComponent {
  @ViewChild('messageInput') messageInputElement!: ElementRef;
  @ViewChild('send_msg') textareaRef!: ElementRef<HTMLTextAreaElement>;

  ngAfterViewInit() {
    if (this.messageInputElement) {
      this.adjustTextareaHeight();
    }
  }

  adjustTextareaHeight(event: Event | null = null) {
    const textarea = event
      ? event.target as HTMLTextAreaElement
      : this.messageInputElement?.nativeElement;

    if (!textarea) return;

    textarea.style.height = 'auto';
    textarea.style.height = (textarea.scrollHeight) + 'px';
  }

  clearTextarea(): void {
    if (this.textareaRef) {
      this.textareaRef.nativeElement.value = '';
      this.textareaRef.nativeElement.style.height = "auto";
    }
  }
}
