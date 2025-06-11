import { Component, Input } from '@angular/core';
import { Message } from '../../../../models/message';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
  selector: 'app-message',
  standalone: true,
  templateUrl: './message.component.html',
  styleUrl: './message.component.scss'
})
export class MessageComponent {
  @Input({ required: true }) message!: Message;

  constructor(private sanitizer: DomSanitizer) { }

  convertMarkdownToHtml(text: string) {
    return this.sanitizer.bypassSecurityTrustHtml(
      this.markdownToHtml(text));
  }

  markdownToHtml(markdown: string): string {
    if (!markdown) return '';

    // Escape HTML characters for safety
    const escapeHtml = (str: string) =>
      str.replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;')
        .replace(/'/g, '&#039;');

    // Sanitize input
    markdown = escapeHtml(markdown);

    // Patterns for Markdown syntax
    const patterns: { [key: string]: RegExp } = {
      // Headers
      '<h1 class="text-4xl font-bold text-gray-900">$1</h1>': /^# (.*)$/gm,
      '<h2 class="text-3xl font-semibold text-gray-800">$1</h2>': /^## (.*)$/gm,
      '<h3 class="text-2xl font-medium text-gray-700">$1</h3>': /^### (.*)$/gm,
      '<h4 class="text-xl font-normal text-gray-600">$1</h4>': /^#### (.*)$/gm,
      '<h5 class="text-lg font-light text-gray-500">$1</h5>': /^##### (.*)$/gm,
      '<h6 class="text-base font-thin text-gray-400">$1</h6>': /^###### (.*)$/gm,

      // Bold
      '<strong>$1</strong>': /\*\*(.*?)\*\*/gm,

      // Italics
      '<em>$1</em>': /\*(.*?)\*/gm,

      // Inline code
      '<code>$1</code>': /`(.*?)`/gm,

      // Blockquotes
      '<blockquote>$1</blockquote>': /^> (.*)$/gm,

      // Links
      '<a href="$2" class="text-blue-500 hover:underline" target="_blank">$1</a>': /\[(.*?)\]\((.*?)\)/gm,

      // Horizontal rule
      '<hr>': /^-{3,}$/gm,

      // Line breaks
      '<br>': /\n/g,
    };

    let html = markdown;

    // Apply replacements for basic Markdown syntax
    for (const [replacement, regex] of Object.entries(patterns)) {
      html = html.replace(regex, replacement);
    }

    // Handle ordered and unordered lists with recursion
    const processList = (text: string, isOrdered: boolean): string => {
      const listTag = isOrdered ? 'ol' : 'ul';
      const listRegex = isOrdered
        ? /^(\d+\.) (.+)$/gm // Match ordered list items
        : /^([-+*]) (.+)$/gm; // Match unordered list items

      let output = '';
      let currentList = '';
      let inList = false;

      text.split('\n').forEach((line) => {
        const match = line.match(listRegex);
        if (match) {
          const item = match[2].trim(); // Remove the list marker from the line
          currentList += `<li>${item}</li>`;
          inList = true;
        } else {
          if (inList) {
            output += `<${listTag} class="p-1">${currentList}</${listTag}>`;
            currentList = '';
            inList = false;
          }
          output += line; // Non-list line
        }
      });

      // Add remaining list items if any
      if (inList) {
        output += `<${listTag}>${currentList}</${listTag}>`;
      }

      return output;
    };

    // Process lists (unordered first, then ordered)
    html = processList(html, false); // Unordered lists
    html = processList(html, true);  // Ordered lists

    // Sanitize and trim final output
    return html.trim();
  }

}
