import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { Skill, SkillFormValue, SkillSyncResult } from '../models/skill.model';

@Injectable({ providedIn: 'root' })
export class SkillService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/skills';

  private readonly _skills = signal<Skill[]>([]);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);

  readonly skills = this._skills.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();

  readonly categories = computed(() =>
    Array.from(new Set(this._skills().map((s) => s.category))).sort()
  );

  async refresh(): Promise<void> {
    this._loading.set(true);
    this._error.set(null);
    try {
      const skills = await firstValueFrom(this.http.get<Skill[]>(this.baseUrl + '/'));
      this._skills.set(skills);
    } catch {
      this._error.set('Could not load skills. Is the API running?');
    } finally {
      this._loading.set(false);
    }
  }

  async getById(id: number): Promise<Skill | undefined> {
    const cached = this._skills().find((s) => s.id === id);
    if (cached) {
      return cached;
    }
    try {
      return await firstValueFrom(this.http.get<Skill>(`${this.baseUrl}/${id}`));
    } catch {
      return undefined;
    }
  }

  async create(value: SkillFormValue): Promise<Skill> {
    const created = await firstValueFrom(this.http.post<Skill>(this.baseUrl + '/', value));
    this._skills.update((skills) => [...skills, created]);
    return created;
  }

  async update(id: number, value: SkillFormValue): Promise<void> {
    await firstValueFrom(this.http.put(`${this.baseUrl}/${id}`, value));
    this._skills.update((skills) =>
      skills.map((s) => (s.id === id ? { ...s, ...value } : s))
    );
  }

  async remove(id: number): Promise<void> {
    await firstValueFrom(this.http.delete(`${this.baseUrl}/${id}`));
    this._skills.update((skills) => skills.filter((s) => s.id !== id));
  }

  async sync(): Promise<SkillSyncResult> {
    const result = await firstValueFrom(
      this.http.post<SkillSyncResult>(this.baseUrl + '/sync', {})
    );
    await this.refresh();
    return result;
  }
}
