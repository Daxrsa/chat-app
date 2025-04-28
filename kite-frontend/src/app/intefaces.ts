export interface User {
    id: string;
    name: string;
    avatar: string;
    status: string;
}

export interface Message {
    id: string;
    sender: string;
    recipient: string;
    text: string;
    timestamp: string;
    status: 'read' | 'delivered' | 'sent';
}

export interface Conversation {
    id: string;
    participants: User[];
    messages: Message[];
}

export interface ConversationResponse {
    conversations: Conversation[];
}