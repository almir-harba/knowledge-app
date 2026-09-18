import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { SkillService } from '../../core/services/skill.service';

const CATEGORIES = ['Backend', 'Frontend', 'DevOps', 'AI', 'Testing', 'Claude Skill', 'Other'];

@Component({
  selector: 'app-skill-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './skill-form.html',
})
export class SkillForm implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly skillService = inject(SkillService);

  protected readonly categories = CATEGORIES;
  protected readonly editingId = signal<number | null>(null);
  protected readonly saving = signal(false);
  protected readonly submitError = signal<string | null>(null);

  protected readonly form = this.fb.nonNullable.group({
    name: ['', Validators.required],
    category: [CATEGORIES[0], Validators.required],
    purpose: ['', Validators.required],
    howBuilt: ['', Validators.required],
    techTags: [''],
  });

  get isEditing(): boolean {
    return this.editingId() !== null;
  }

  async ngOnInit(): Promise<void> {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    this.editingId.set(id);

    if (this.skillService.skills().length === 0) {
      await this.skillService.refresh();
    }

    const skill = await this.skillService.getById(id);
    if (skill) {
      this.form.patchValue({
        name: skill.name,
        category: skill.category,
        purpose: skill.purpose,
        howBuilt: skill.howBuilt,
        techTags: skill.techTags.join(', '),
      });
    }
  }

  async submit(): Promise<void> {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();
    const value = {
      name: raw.name.trim(),
      category: raw.category,
      purpose: raw.purpose.trim(),
      howBuilt: raw.howBuilt.trim(),
      techTags: raw.techTags
        .split(',')
        .map((tag) => tag.trim())
        .filter((tag) => tag.length > 0),
    };

    this.saving.set(true);
    this.submitError.set(null);

    try {
      const id = this.editingId();
      if (id !== null) {
        await this.skillService.update(id, value);
        await this.router.navigate(['/skills', id]);
      } else {
        const created = await this.skillService.create(value);
        await this.router.navigate(['/skills', created.id]);
      }
    } catch {
      this.submitError.set('Something went wrong saving this skill. Please try again.');
    } finally {
      this.saving.set(false);
    }
  }
}
