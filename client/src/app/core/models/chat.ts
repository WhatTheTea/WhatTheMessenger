import { guid } from '../../primitives';
import { Message } from './message';
import { User } from './user';

export interface Chat {
    id: guid;
    name: string;
    users: User[];
    messages: Message[];
}