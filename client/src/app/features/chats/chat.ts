import { guid } from '../../primitives';
import { User } from '../users/user';
import { Message } from './message';

export interface Chat {
    id: guid;
    name: string;
    users: User[];
    messages: Message[];
}