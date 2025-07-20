export interface User {
  id: string;
  firstName: string;
  lastName: string;
  userName: string;
  email: string;
  role: string;
  profilePicture: string;
  createdAt: string;
  token: string;
}

export interface Message {
  id: string;
  sender: string;
  recipient: string;
  text: string;
  timestamp: string;
  status: 'read' | 'delivered' | 'sent';
}

interface Conversation {
  id: string;
  participants: User[];
  messages: Message[];
}

export interface ConversationResponse {
  conversations: Conversation[];
}

interface Friend {
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

interface Contact {
  id: number;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  username: string;
  profilePicture: string;
  status: string;
  lastSeen: string;
}

export interface ContactsResponse {
  contacts: Contact[];
}
