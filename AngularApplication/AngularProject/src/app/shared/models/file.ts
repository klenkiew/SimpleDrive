import {User} from "./user";

export class File {
  id: string;
  name: string;
  size: number;
  mimeType: string;
  description: string;
  owner: User;
  dateCreated: Date;

  get ownerName()
  {
    return this.owner.username;
  }

  constructor(id: string, name: string, size: number, description: string, mimeType: string, owner: User,
              dateCreated: Date) {
    this.id = id;
    this.name = name;
    this.size = size;
    this.mimeType = mimeType;
    this.owner = owner;
    this.description = description;
    this.dateCreated = dateCreated;
  }
}
