import { guid } from '../../primitives';

export interface User {
  id: guid;
  username: string;
  displayName: string;
  chatIds: guid[];
}
