import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Feature } from '../../core/_models/entity/feature';
import { SiteService } from '../_services/site.service';

@Component({
  selector: 'app-feature',
  templateUrl: './feature.component.html',
  styleUrls: ['./feature.component.css']
})
export class FeatureComponent implements OnInit {

  constructor(private siteService: SiteService) { }

  features: Feature[];

  ngOnInit() {
    // get features from service
    this.siteService.getFeatures().then(features => {
      // order features by 'order' member
      this.features = features.sort((a: Feature, b: Feature) => a.order - b.order);
    });

  }
  
}
