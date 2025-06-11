import { Component, ViewChild, ElementRef, AfterViewChecked, ChangeDetectorRef } from '@angular/core';
import { AssistantService } from '../../services/assistant.service';
import { Message } from '../../models/message';
import { FormsModule } from '@angular/forms';
import { BodyComponent, FooterComponent, HeaderComponent } from '..';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [FormsModule, HeaderComponent, BodyComponent, FooterComponent],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent implements AfterViewChecked {
  @ViewChild(BodyComponent) private bodyComponent!: BodyComponent;

  isVisible = true;
  isLoading = false;
  chatId = '';
  file!: File | null;
  conversation: Message[] = [];
  private isScrolledToBottom = true;
  private shouldScrollToBottom = false;

  constructor(
    private assistantService: AssistantService,
    private changeDetectorRef: ChangeDetectorRef
  ) {
    this.isLoading = true;

    this.conversation.push({
      status: 'Loading',
      role: 'Assistant',
      type: 'Message',
      text: 'Hello! How can I assist you?'
    });

    this.assistantService.startSession().subscribe((response) => {
      this.chatId = response.chatId;

      this.conversation[this.conversation.length - 1] = {
        status: 'Success',
        role: 'Assistant',
        type: 'Message',
        text: response.message
      };

      this.isLoading = false;
      this.shouldScrollToBottom = true;
      this.changeDetectorRef.detectChanges();
    });
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  private scrollToBottom(): void {
    try {
      if (this.isScrolledToBottom) {
        this.bodyComponent.scrollToBottom();
      }
    } catch (err) { }
  }

  onScroll(event: Event): void {
    const element = event.target as HTMLElement;
    const atBottom = Math.abs(element.scrollHeight - element.scrollTop - element.clientHeight) < 1;
    this.isScrolledToBottom = atBottom;
  }

  attachClicked(event: File) {
    this.file = event;
  }

  sendMessage(event: any) {
    this.isLoading = true;

    if (this.file) {
      this.conversation.push({
        status: 'Success',
        role: 'User',
        type: 'File',
        text: this.file.name
      });
    }

    if (event) {
      this.conversation.push({
        status: 'Success',
        role: 'User',
        type: 'Message',
        text: event
      });
    }

    this.conversation.push({
      status: 'Loading',
      role: 'Assistant',
      type: 'Message',
      text: event
    });

    this.shouldScrollToBottom = true;
    this.changeDetectorRef.detectChanges();

    this.assistantService.sendUserMessage({ chatId: this.chatId, message: event, cvFile: this.file }).subscribe((response) => {
      this.conversation[this.conversation.length - 1] = {
        status: 'Success',
        role: 'Assistant',
        type: 'File',
        text: response.message
      };

      this.isLoading = false;
      this.shouldScrollToBottom = true;
      this.changeDetectorRef.detectChanges();
    });

    this.file = null;
  }
}