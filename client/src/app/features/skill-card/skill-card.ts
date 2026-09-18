import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Skill } from '../../core/models/skill.model';

const CATEGORY_COLORS: Record<string, string> = {
  Backend: 'bg-emerald-100 text-emerald-800',
  Frontend: 'bg-sky-100 text-sky-800',
  DevOps: 'bg-amber-100 text-amber-800',
  AI: 'bg-violet-100 text-violet-800',
  Testing: 'bg-rose-100 text-rose-800',
  'Claude Skill': 'bg-orange-100 text-orange-800',
};
const DEFAULT_COLOR = 'bg-slate-100 text-slate-800';

@Component({
  selector: 'app-skill-card',
  imports: [RouterLink],
  templateUrl: './skill-card.html',
})
export class SkillCard {
  readonly skill = input.required<Skill>();

  categoryColor(category: string): string {
    return CATEGORY_COLORS[category] ?? DEFAULT_COLOR;
  }
}
