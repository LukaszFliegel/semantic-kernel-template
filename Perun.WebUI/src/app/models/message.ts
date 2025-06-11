export interface Message {
    status: 'Failed' | 'Success' | 'Loading';
    role: 'System' | 'User' | 'Assistant' | 'Tool' | 'Function';
    type: 'Message' | 'File';
    text: string;
}