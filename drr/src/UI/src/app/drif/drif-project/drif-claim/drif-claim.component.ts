import { Component, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'drr-drif-claim',
  standalone: true,
  imports: [],
  templateUrl: './drif-claim.component.html',
  styleUrl: './drif-claim.component.scss',
})
export class DrifClaimComponent {
  route = inject(ActivatedRoute);

  projectId?: string;
  claimId?: string;

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      this.projectId = params.get('projectId')!;
      this.claimId = params.get('claimId')!;
    });
  }
}
