import { guid } from '../../primitives';

export interface CreateChat {
  name: string;
  participants: guid[];
}
