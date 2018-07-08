import {User} from "./user";

export class File {
  id: string;
  name: string;
  size: number;
  description: string;
  owner: User;
  lastModified: Date;

  get ownerName()
  {
    return this.owner.username;
  }

  constructor(id: string, name: string, size: number, description: string, owner: User, lastModified: Date) {
    this.id = id;
    this.name = name;
    this.size = size;
    this.owner = owner;
    this.description = description;
    this.lastModified = lastModified;
  }
}
