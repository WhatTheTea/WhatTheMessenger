import { guid } from '../../primitives';
import { Message } from './message';
import { User } from '../auth/user';

export interface Chat {
    id: guid;
    name: string;
    users: User[];
    messages: Message[];
}