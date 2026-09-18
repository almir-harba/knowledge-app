import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SkillService } from '../../core/services/skill.service';
import { SkillCard } from '../skill-card/skill-card';

@Component({
  selector: 'app-skill-list',
  imports: [FormsModule, RouterLink, SkillCard],
  templateUrl: './skill-list.html',
})
export class SkillList implements OnInit {
  protected readonly skillService = inject(SkillService);

  protected readonly search = signal('');
  protected readonly selectedCategory = signal('All');
  protected readonly syncing = signal(false);
  protected readonly syncMessage = signal<string | null>(null);

  protected readonly filteredSkills = computed(() => {
    const term = this.search().trim().toLowerCase();
    const category = this.selectedCategory();

    return this.skillService.skills().filter((skill) => {
      const matchesCategory = category === 'All' || skill.category === category;
      const matchesSearch =
        !term ||
        skill.name.toLowerCase().includes(term) ||
        skill.purpose.toLowerCase().includes(term) ||
        skill.techTags.some((tag) => tag.toLowerCase().includes(term));
      return matchesCategory && matchesSearch;
    });
  });

  ngOnInit(): void {
    void this.skillService.refresh();
  }

  async syncSkills(): Promise<void> {
    this.syncing.set(true);
    this.syncMessage.set(null);
    try {
      const result = await this.skillService.sync();
      this.syncMessage.set(
        `Synced: ${result.importedFromDisk} imported from disk, ${result.exportedToDisk} written to disk.`
      );
    } catch {
      this.syncMessage.set('Sync failed. Is the API running?');
    } finally {
      this.syncing.set(false);
    }
  }
}
