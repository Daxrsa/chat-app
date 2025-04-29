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

export interface Friend {
    id: number;
    name: string;
    status: 'online' | 'offline' | 'away' | 'busy';
    statusMessage: string;
    avatar: string;
    lastMessage: string;
    timestamp: string;
    unread: number;
}

export interface FriendsResponse {
    friends: Friend[];
}