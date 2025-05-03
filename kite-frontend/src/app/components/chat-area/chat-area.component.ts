import { Component, ElementRef, ViewChild } from '@angular/core';
import { MessagesService } from '../../services/messages.service';
import { CommonModule } from '@angular/common';
import { ConversationResponse } from '../../intefaces';

@Component({
  selector: 'app-chat-area',
  imports: [CommonModule],
  templateUrl: './chat-area.component.html',
  styleUrl: './chat-area.component.css'
})
export class ChatAreaComponent {
  @ViewChild('messageInput') messageInputElement!: ElementRef;
  @ViewChild('send_msg') textareaRef!: ElementRef<HTMLTextAreaElement>;

  conversations: ConversationResponse[] = [];

  constructor(
    private messageService: MessagesService
  ) {} 

  ngOnInit(): void {
    this.messageService.getMessages().subscribe({
      next: (data: ConversationResponse[]) => {
        this.conversations = data;
        console.log('Conversations received:', this.conversations);
      },
      error: (err) => {
        console.error('Error fetching conversations:', err);
      },
    });
  }

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
