import { Component } from '@angular/core';
import { TestBuilderService } from '../services/TestBuilder';
@Component({
  selector: 'app-tests',
  templateUrl: './tests.component.html',
  styleUrls: ['./tests.component.scss']
})
export class TestsComponent {
  constructor(private testBuilder: TestBuilderService) {
  }

  loadTestFromLocalStorage() {

  }

  loadTestFromFile(){

  }

}
