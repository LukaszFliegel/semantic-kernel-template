export interface MessageRequest {
    chatId: string;
    message: string | null;
    cvFile: File | null;
}