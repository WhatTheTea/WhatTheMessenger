import { guid } from "../../primitives";

export interface SendMessage {
    senderId : guid, 
    chatId : guid, 
    content : string
}