export interface Skill {
  id: number;
  name: string;
  category: string;
  purpose: string;
  howBuilt: string;
  techTags: string[];
  createdAt: string;
  slug: string;
}

export interface SkillSyncResult {
  importedFromDisk: number;
  exportedToDisk: number;
}

export interface SkillFormValue {
  name: string;
  category: string;
  purpose: string;
  howBuilt: string;
  techTags: string[];
}
