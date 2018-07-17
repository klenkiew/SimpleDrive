import {User} from "./user";

export class File {
  id: string;
  fileName: string;
  size: number;
  mimeType: string;
  description: string;
  owner: User;
  dateCreated: Date;
  dateModified: Date;

  constructor(id: string, name: string, size: number, description: string, mimeType: string, owner: User,
              dateCreated: Date, dateModified: Date) {
    this.id = id;
    this.fileName = name;
    this.size = size;
    this.mimeType = mimeType;
    this.owner = owner;
    this.description = description;
    this.dateCreated = dateCreated;
    this.dateModified = dateModified;
  }
}
