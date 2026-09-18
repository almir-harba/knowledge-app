import { DatePipe } from '@angular/common';
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Skill } from '../../core/models/skill.model';
import { SkillService } from '../../core/services/skill.service';

@Component({
  selector: 'app-skill-detail',
  imports: [RouterLink, DatePipe],
  templateUrl: './skill-detail.html',
})
export class SkillDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly skillService = inject(SkillService);

  protected readonly skill = signal<Skill | undefined>(undefined);
  protected readonly notFound = signal(false);
  protected readonly deleting = signal(false);

  async ngOnInit(): Promise<void> {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (this.skillService.skills().length === 0) {
      await this.skillService.refresh();
    }

    const skill = await this.skillService.getById(id);
    if (skill) {
      this.skill.set(skill);
    } else {
      this.notFound.set(true);
    }
  }

  async deleteSkill(): Promise<void> {
    const skill = this.skill();
    if (!skill || !confirm(`Delete "${skill.name}"? This cannot be undone.`)) {
      return;
    }

    this.deleting.set(true);
    await this.skillService.remove(skill.id);
    await this.router.navigate(['/']);
  }
}
